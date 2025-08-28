using Hardware_Monitor.Model;
using Hardware_Monitor.MVVM;
using LibreHardwareMonitor.Hardware;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Threading;

namespace Hardware_Monitor.ViewModel
{

    public class StaticSensorButton : ViewModelBase
    {
        private string label;

        public string Label
        {
            get { return label; }
            set { label = value;
                OnPropertyChanged();
            }
        }

        private string sensorValue;

        public string SensorValue
        {
            get { return sensorValue; }
            set { sensorValue = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand Command { get; set; }
    }

    class MainWindowViewModel : ViewModelBase
    {
        private readonly DispatcherTimer _timer;
        private Computer _computer;

        // Cached sensors
        private ISensor? _cpuTempSensor;
        private ISensor? _gpuTempSensor;
        private ISensor? _memoryUsageSensor;

        public ObservableCollection<object> SensorButtons { get; set; } = new ObservableCollection<object>();

        public RelayCommand AddSensorCommand => new RelayCommand(param => AddSensor(param as SensorViewModel));
        public RelayCommand RemoveSensorCommand => new RelayCommand(param => RemoveSensor(param as SensorViewModel));

        private void AddSensor(SensorViewModel sensor)
        {
            SensorButtons.Add(sensor);
        }
        private void RemoveSensor(SensorViewModel sensor)
        {
            SensorButtons.Remove(sensor);
        }



        public ObservableCollection<TypeSensorViewModel> CpuSensorTypes { get; set; }
        public ObservableCollection<TypeSensorViewModel> GpuSensorTypes { get; set; }
        public ObservableCollection<TypeSensorViewModel> MemorySensorTypes { get; set; }

        public RelayCommand OpenCpu => new RelayCommand(_ => ToggleTree("CPU"));
        public RelayCommand OpenGpu => new RelayCommand(_ => ToggleTree("GPU"));
        public RelayCommand OpenMemory => new RelayCommand(_ => ToggleTree("Memory"));

        private void ToggleTree(string which)
        {
            CpuTreeVisibility = which == "CPU" ?
                (CpuTreeVisibility == "Visible" ? "Collapsed" : "Visible") : CpuTreeVisibility;

            GpuTreeVisibility = which == "GPU" ?
                (GpuTreeVisibility == "Visible" ? "Collapsed" : "Visible") : GpuTreeVisibility;

            MemoryTreeVisibility = which == "Memory" ?
                (MemoryTreeVisibility == "Visible" ? "Collapsed" : "Visible") : MemoryTreeVisibility;

            if(CpuTreeVisibility == "Collapsed" && GpuTreeVisibility == "Collapsed" && MemoryTreeVisibility == "Collapsed")
                TreeVisibility = "Collapsed";
            else
                TreeVisibility = "Visible";

        }
        
        private string _treeVisibility = "Collapsed";

        public string TreeVisibility
        {
            get { return _treeVisibility; }
            set { _treeVisibility = value;
                OnPropertyChanged();
            }
        }


        private string _cpuTreeVisibility = "Collapsed";
        public string CpuTreeVisibility
        {
            get => _cpuTreeVisibility;
            set { _cpuTreeVisibility = value; OnPropertyChanged(); }
        }

        private string _gpuTreeVisibility = "Collapsed";
        public string GpuTreeVisibility
        {
            get => _gpuTreeVisibility;
            set { _gpuTreeVisibility = value; OnPropertyChanged(); }
        }

        private string _memoryTreeVisibility = "Collapsed";
        public string MemoryTreeVisibility
        {
            get => _memoryTreeVisibility;
            set { _memoryTreeVisibility = value; OnPropertyChanged(); }
        }

        private StaticSensorButton Cpu;
        private StaticSensorButton Gpu;
        private StaticSensorButton Mem;


        public MainWindowViewModel()
        {
            // Populate sensorbuttons with static buttons
            Cpu = new StaticSensorButton { Label = "CPU Temp", SensorValue = "0", Command = OpenCpu };
            Gpu = new StaticSensorButton { Label = "GPU Temp", SensorValue = "0", Command = OpenGpu };
            Mem = new StaticSensorButton { Label = "Memory Usage", SensorValue = "0", Command = OpenMemory };
            SensorButtons.Add(Cpu);
            SensorButtons.Add(Gpu);
            SensorButtons.Add(Mem);


            // Initialize LibreHardwareMonitor
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
            };
            _computer.Open();
            _computer.Accept(new UpdateVisitor());

            CpuSensorTypes = new ObservableCollection<TypeSensorViewModel>();
            GpuSensorTypes = new ObservableCollection<TypeSensorViewModel>();
            MemorySensorTypes = new ObservableCollection<TypeSensorViewModel>();
            CacheSensors();

            // Update sensors
            Monitor();
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _timer.Tick += (s, e) => Monitor();
            _timer.Start();
        }

        private void CacheSensors()
        {
            // Clear all collections before repopulating
            CpuSensorTypes.Clear();
            GpuSensorTypes.Clear();
            MemorySensorTypes.Clear();

            foreach (IHardware hardware in _computer.Hardware)
            {
                hardware.Update();

                // Prepare a dictionary to group sensors by type
                var typeSensorDict = new Dictionary<string, TypeSensor>();

                foreach (ISensor sensor in hardware.Sensors)
                {
                    // Cache special sensors for quick access
                    if (_cpuTempSensor == null && hardware.HardwareType == HardwareType.Cpu && sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Max"))
                        _cpuTempSensor = sensor;
                    if (_gpuTempSensor == null && hardware.HardwareType == HardwareType.GpuNvidia && sensor.SensorType == SensorType.Temperature && sensor.Name.Contains("Core"))
                        _gpuTempSensor = sensor;
                    if (_memoryUsageSensor == null && hardware.HardwareType == HardwareType.Memory && sensor.SensorType == SensorType.Load && !hardware.Name.Contains("Virtual"))
                        _memoryUsageSensor = sensor;

                    // Group sensors by their type
                    string typeName = sensor.SensorType.ToString();
                    if (!typeSensorDict.TryGetValue(typeName, out var typeSensor))
                    {
                        typeSensor = new TypeSensor(typeName);
                        typeSensorDict[typeName] = typeSensor;
                    }
                    typeSensor.AddSensor(sensor.Name, sensor);
                }

                // Map hardware type to the correct collection
                var viewModelCollection = hardware.HardwareType switch
                {
                    HardwareType.Cpu => CpuSensorTypes,
                    HardwareType.GpuNvidia => GpuSensorTypes,
                    HardwareType.Memory => MemorySensorTypes,
                    _ => null
                };

                if (viewModelCollection != null)
                {
                    foreach (var typeSensor in typeSensorDict.Values)
                        viewModelCollection.Add(new TypeSensorViewModel(typeSensor));
                }
            }
        }

        public void Monitor()
        {
            _computer.Accept(new UpdateVisitor());

            Cpu.SensorValue = _cpuTempSensor?.Value.HasValue == true ? $"{_cpuTempSensor.Value.Value:0.00}°C" : "-";
            Gpu.SensorValue = _gpuTempSensor?.Value.HasValue == true ? $"{_gpuTempSensor.Value.Value:0.00}°C" : "-";
            Mem.SensorValue = _memoryUsageSensor?.Value.HasValue == true ? $"{_memoryUsageSensor.Value.Value:0.00}%" : "-";

            if(CpuTreeVisibility == "Visible")
                UpdateSensorValues(CpuSensorTypes);

            if(GpuTreeVisibility == "Visible")
                UpdateSensorValues(GpuSensorTypes);

            if(MemoryTreeVisibility == "Visible")
                UpdateSensorValues(MemorySensorTypes);


        }

        public void UpdateSensorValues(ObservableCollection<TypeSensorViewModel> sensorTypes)
        {
            foreach (var typeSensorVM in sensorTypes)
            {
                foreach (var sensorVM in typeSensorVM.Sensors)
                {
                    sensorVM.UpdateValue();
                }
            }
        }

        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer) { computer.Traverse(this); }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
    }
}

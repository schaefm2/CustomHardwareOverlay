using Hardware_Monitor.MVVM;
using Hardware_Monitor.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hardware_Monitor.ViewModel
{
    public class TypeSensorViewModel : ViewModelBase
    {   
        private readonly TypeSensor _typeSensor;

        public TypeSensorViewModel(TypeSensor typeSensor)
        {
            _typeSensor = typeSensor;
            Sensors = new ObservableCollection<SensorViewModel>(
                _typeSensor.Sensors.Select(s => new SensorViewModel(s))
            );

            //// Optionally, subscribe to collection changes if sensors can be added/removed at runtime
            //_typeSensor.Sensors.CollectionChanged += (s, e) =>
            //{
            //    if (e.NewItems != null)
            //    {
            //        foreach (Sensor sensor in e.NewItems)
            //            Sensors.Add(new SensorViewModel(sensor));
            //    }
            //    if (e.OldItems != null)
            //    {
            //        foreach (Sensor sensor in e.OldItems)
            //        {
            //            var vmToRemove = Sensors.FirstOrDefault(vm => vm.Model == sensor);
            //            if (vmToRemove != null)
            //                Sensors.Remove(vmToRemove);
            //        }
            //    }
            //};
        }

        public string Name
        {
            get => _typeSensor.Name;
            set
            {
                if (_typeSensor.Name != value)
                {
                    _typeSensor.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<SensorViewModel> Sensors { get; }

        public TypeSensor Model => _typeSensor;
    }
}

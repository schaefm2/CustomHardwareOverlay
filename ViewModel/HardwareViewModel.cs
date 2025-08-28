using Hardware_Monitor.Model;
using Hardware_Monitor.MVVM;
using System.Collections.ObjectModel;

namespace Hardware_Monitor.ViewModel
{
    internal class HardwareViewModel : ViewModelBase
    {
        private readonly Hardware _hardware;


        public HardwareViewModel(Hardware hardware)
        {
            _hardware = hardware;
            SensorTypes = new ObservableCollection<TypeSensorViewModel>(
                _hardware.SensorTypes.Select(st => new TypeSensorViewModel(st))
            );
        }


        public string Name
        {
            get => _hardware.Name;
            set
            {
                if (_hardware.Name != value)
                {
                    _hardware.Name = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<TypeSensorViewModel> SensorTypes { get; }

        public Hardware Model => _hardware;
    }
}

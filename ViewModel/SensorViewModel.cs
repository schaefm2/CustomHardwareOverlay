using Hardware_Monitor.MVVM;
using Hardware_Monitor.Model;

namespace Hardware_Monitor.ViewModel
{
    public class SensorViewModel : ViewModelBase
    {
        private readonly Sensor _sensor;

        public SensorViewModel(Sensor sensor)
        {
            _sensor = sensor;
        }

        public string Name
        {
            get => _sensor.Name;
            set
            {
                if (_sensor.Name != value)
                {
                    _sensor.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Value
        {
            get => _sensor.Value;
            private set
            {
                if (_sensor.Value != value)
                {
                    _sensor.Value = value;
                    OnPropertyChanged();
                }
            }
        }

        public void UpdateValue()
        {
            string oldValue = _sensor.Value;
            _sensor.UpdateValue();
            // Notify only if value changed
            if (_sensor.Value != oldValue)
            {
                OnPropertyChanged(nameof(Value));
            }
        }

        public Sensor Model => _sensor;     
    }
}

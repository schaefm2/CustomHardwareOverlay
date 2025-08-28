using System.Collections.ObjectModel;

namespace Hardware_Monitor.Model
{
    internal class Hardware
    {
        public string Name { get; set; }

        public ObservableCollection<TypeSensor> SensorTypes { get; set; }

        public Hardware(string name)
        {
            Name = name;
            SensorTypes = new ObservableCollection<TypeSensor>();
        }

        public void AddSensorType(string typeName)
        {
            SensorTypes.Add(new TypeSensor(typeName));
        }
    }
}

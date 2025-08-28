using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hardware_Monitor.Model
{
    public class TypeSensor
    {
        public string Name { get; set; }

        public ObservableCollection<Sensor> Sensors { get; set; }

        public TypeSensor(string name)
        {
            Name = name;
            Sensors = new ObservableCollection<Sensor>();
        }

        public void AddSensor(string typeName, ISensor sensor)
        {
            Sensors.Add(new Sensor(typeName, sensor));
        }
    }
}

using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hardware_Monitor.Model
{
    public class Sensor
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ISensor LibreSensor { get; set; }
        public Sensor(string name, ISensor libreSensor)
        {
            Name = name;
            Value = "N/A";
            LibreSensor = libreSensor;
        }

        public void UpdateValue()
        {

            // Determine the format based on sensor type
            if (LibreSensor.SensorType == SensorType.Temperature)
                Value = LibreSensor.Value.HasValue == true ? $"{LibreSensor.Value.Value:0.00}°C" : "-";
            else if (LibreSensor.SensorType == SensorType.Voltage)
                Value = LibreSensor.Value.HasValue == true ? $"{LibreSensor.Value.Value:0.00}V" : "-";
            else if (LibreSensor.SensorType == SensorType.Fan)
                Value = LibreSensor.Value.HasValue == true ? $"{LibreSensor.Value.Value:0.00} RPM" : "-";
            else if (LibreSensor.SensorType == SensorType.Clock)
                Value = LibreSensor.Value.HasValue == true ? $"{LibreSensor.Value.Value:0.00} MHz" : "-";
            else if (LibreSensor.SensorType == SensorType.Power)
                Value = LibreSensor.Value.HasValue == true ? $"{LibreSensor.Value.Value:0.00} W" : "-";
            else if (LibreSensor.SensorType == SensorType.Data)
                Value = LibreSensor.Value.HasValue == true ? $"{LibreSensor.Value.Value:0.00} GB" : "-";
            else if (LibreSensor.SensorType == SensorType.Load)
                Value = LibreSensor.Value.HasValue == true ? $"{LibreSensor.Value.Value:0.00}%" : "-";
            else
                Value = LibreSensor.Value.HasValue == true ? $"{LibreSensor.Value.Value:0.00}" : "-";

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Model
{
    public class Sensor : GenericObjectBase
    {
        public Sensor()
        {

        }

        public Sensor(string id, string name, string displayName)
            : base(id, name, displayName)
        {
        }

        public string HardId { get; set; }
        public string MeasureUnit { get; set; }
        public SensorType SensorType { get; set; }
        public string Value { get; set; }
        public DateTime? MeasureTime { get; set; }

    }

    public enum SensorType
    {
        Temperature,
        Pressure,
        Moisture,
        Level,
        State
    }

}
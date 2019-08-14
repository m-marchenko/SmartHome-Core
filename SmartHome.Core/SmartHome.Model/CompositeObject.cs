using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartHome.Model
{
    public class CompositeObject : GenericObjectBase
    {
        public CompositeObject()
        {
        }

        public CompositeObject(string id, string name, string displayName) 
            : base(id, name, displayName)
        {
        }

        public List<CompositeObject> Units { get; set; }

        public List<Sensor> Sensors { get; set; }

        public List<Command> Commands { get; set; }
        
        public Sensor FindSensor(string sensorId)
        {
            if (Sensors == null)
                return null;

            var result = Sensors.Where(s => s.HardId == sensorId).FirstOrDefault();
            if (result == null)
            {
                foreach (var unit in Units)
                {
                    result = unit.FindSensor(sensorId);

                    if (result != null)
                        break;
                }
            }

            return result;
        }

        public CompositeObject FindUnit(string unitId)
        {
            if (Units == null)
            {
                return null;
            }

            var result = Units.Where(s => s.ClientId == unitId).FirstOrDefault();

            if (result == null)
            {
                foreach (var unit in Units)
                {
                    result = unit.FindUnit(unitId);

                    if (result != null)
                        break;
                }
            }

            return result;
        }

        public void UpdateParents()
        {
            if (Units != null && Units.Any())
            {
                Units.ForEach(u => {
                    u.Parent = this;
                    u.UpdateParents();
                });
            }

            if (Sensors != null && Sensors.Any())
            {
                Sensors.ForEach(s => s.Parent = this);
            }

            if(Commands != null && Commands.Any())
            {
                Commands.ForEach(c => c.Parent = this);
            }
        }
    }
}

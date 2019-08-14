﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SmartHome.Models
{

    #region Interfaces

    public interface IRootObject : ICompositeObject
    {
        ISensor FindSensor(string sensorId);

        ICompositeObject FindCompositeObject(string id);
    }

    public interface IGenericObject
    {
        string Id { get; }        

        string Name { get; }

        string DisplayName { get; }

        string ClientId { get; }

        string HardId { get; }

        IGenericObject Parent { get; set; }

    }

    public interface ICompositeObject : IGenericObject
    {
        ICollection<ISensor> Sensors { get; }

        ICollection<IGenericObject> All { get; }

        ICollection<IControlUnit> ControlUnits { get; }

        ICollection<ICompositeObject> GetCompositeObjects(bool includeConrolUnits = true);

        ICompositeObject AddSensor(ISensor sensor);

        ICompositeObject AddCompositeObject(ICompositeObject cobject);

        ICompositeObject AddControlUnit(IControlUnit unit);
        
    }

    public interface ISensor : IGenericObject
    {
        SensorType SensorType { get; }

        string MeasureUnit { get; }

        string Value { get; set; }

        DateTime MeasureTime { get; set; }
    }


    public interface IControlUnit : ICompositeObject
    {
        List<ICommand> AvailableCommands { get; }

        Dictionary<string, string> Settings { get; }

        void ExecuteCommand(ICommand command);
    }


    public interface ICommand : IGenericObject
    {        
        bool CanExecute { get; set; }
    }

    #endregion

    #region Enums

    public enum SensorType
    {
        Temperature,
        Pressure,
        Moisture,
        Level,
        State
    }

    #endregion

    #region Implementations

    [Serializable]
    public abstract class GenericObjectBase : IGenericObject
    {
        protected GenericObjectBase()
        {

        }
        protected GenericObjectBase(string id, string name, string displayName)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
        }

        public string ClientId
        {
            get
            {
                var result = Id;
                var p = Parent;
                while (p != null)
                {
                    result = String.Format("{0}_{1}", p.Id, result);
                    p = p.Parent;
                }

                return result;
            }
        }

        public string DisplayName
        {
            get;  set;
        }

        public string Id
        {
            get;  set;
        }

        public string Name
        {
            get;  set;
        }

        [XmlIgnore]
        [JsonIgnore]
        public IGenericObject Parent
        {
            get; set;
        }

        /// <summary>
        /// это "железный" id. уникальный в рамках всего проекта
        /// </summary>
        // временное решение
        public string HardId { get { return Id; } }
    }

    [Serializable]
    public abstract class CompositeObjectBase : GenericObjectBase, ICompositeObject
    {
        protected readonly ICollection<IGenericObject> _all = new List<IGenericObject>();

        protected CompositeObjectBase() : base()
        {
        }

        protected CompositeObjectBase(string id, string name, string displayName) : base(id, name, displayName)
        {
        }

        public ICollection<IGenericObject> All { get { return _all; } }
        public ICollection<ISensor> Sensors { get { return _all.OfType<ISensor>().ToList(); } }

        public ICollection<IControlUnit> ControlUnits
        {
            get
            {
                return _all.OfType<IControlUnit>().ToList();
            }
        }

        protected void AddSensorInternal(ISensor sensor)
        {
            _all.Add(sensor);
            sensor.Parent = this;
        }

        protected void AddCompositeObjectInternal(ICompositeObject obj)
        {
            _all.Add(obj);
            obj.Parent = this;
        }

        protected void AddContolUnitInternal(IControlUnit unit)
        {
            _all.Add(unit);
            unit.Parent = this;
        }

        public ICollection<ICompositeObject> GetCompositeObjects(bool includeConrolUnits = true)
        {
            var query = _all.OfType<ICompositeObject>();

            if (!includeConrolUnits)
                query = query.Where(c => !(c is IControlUnit));

            return query.ToList();
        }

        public ICompositeObject AddSensor(ISensor sensor)
        {
            AddSensorInternal(sensor);
            return this;
        }

        public ICompositeObject AddCompositeObject(ICompositeObject cobject)
        {
            AddCompositeObjectInternal(cobject);
            return this;
        }

        public ICompositeObject AddControlUnit(IControlUnit unit)
        {
            AddContolUnitInternal(unit);
            return this;
        }
    }

    [Serializable]
    public class Fazenda : CompositeObjectBase, IRootObject
    {
        public Fazenda()
            : base()
        {
            Id = "root";
            Name = "Fazenda";
            DisplayName = "Фазенда";

            #region Объекты недвижимости

            this
                // Дом
                .AddCompositeObject(new House()
                                        .AddSensor(new TemperatureSensor("battery", "battery", "температура батареи") { Value = "63" })
                                        .AddSensor(new TemperatureSensor("firstfloor", "firstfloor", "температура на 1 этаже"))
                                        .AddSensor(new TemperatureSensor("secondfloor", "secondfloor", "температура на 2 этаже")))
                // Гараж
                .AddCompositeObject(new Garage("garage", "garage", "Гараж"))
                // Участок
                .AddCompositeObject(new Garden("garden", "garden", "Участок")
                                        .AddSensor(new MovementSensor("movesensor", "movesensor", "датчик движения на гараже")))  // Датчик движения

                // Теплица 1
                .AddCompositeObject(new GreenHouse("parnik1", "parnik1", "Теплица 1")
                                        .AddSensor(new TemperatureSensor("temperature", "temperature", "температура"))
                                        .AddSensor(new StateSensor("door", "door", "состояние двери") { Value = "закрыта" })
                                        .AddCompositeObject(new Barrel("barrel1", "barrel1", "Бочка 1")
                                                                .AddSensor(new LevelSensor("level", "level", "уровень")))
                                        .AddCompositeObject(new Barrel("barrel2", "barrel2", "Бочка 2")
                                                                .AddSensor(new LevelSensor("level", "level", "уровень")))

                                                                )

                // Теплица 2
                .AddCompositeObject(new GreenHouse("parnik2", "parnik2", "Теплица 2")
                                        .AddCompositeObject(new Barrel("barrel1", "barrel1", "Бочка 1")
                                                                .AddSensor(new LevelSensor("level", "level_barrel1_parnik2", "уровень")))
                                        .AddSensor(new TemperatureSensor("temperature", "temperature", "температура"))
                                        .AddSensor(new StateSensor("door", "door", "состояние двери") { Value = "закрыта" })

                );

            #endregion

            #region Датчики


            #endregion
        }

        public ICompositeObject FindCompositeObject(string id)
        {
            return FindCompositeObject(id, this);
        }

        private ICompositeObject FindCompositeObject(string id, ICompositeObject obj)
        {
            var result = obj.GetCompositeObjects().Where(s => s.ClientId == id).FirstOrDefault();

            if (result == null)
            {
                foreach (var cobj in obj.GetCompositeObjects())
                {
                    result = FindCompositeObject(id, cobj);
                    if (result != null)
                        break;
                }
            }

            return result;

        }



        public ISensor FindSensor(string sensorId)
        {
            return FindSensor(sensorId, this);
        }

        private ISensor FindSensor(string id, ICompositeObject cobject)
        {
            var result = cobject.Sensors.Where(s => s.HardId == id).FirstOrDefault();

            if (result == null)
            {
                foreach (var cobj in cobject.GetCompositeObjects())
                {
                    result = FindSensor(id, cobj);
                    if (result != null)
                        break;
                }
            }

            return result;
        }
    }

    [Serializable]
    public class House : CompositeObjectBase
    {
        public House()
        {
            Id = "House";
            Name = "House";
            DisplayName = "Дом";
        }
    }

    [Serializable]
    public class Garage : CompositeObjectBase
    {
        // ids starting from 40
        public Garage(string id, string name, string displayName)
            : base(id, name, displayName)
        {

        }
    }

    [Serializable]
    public class Garden : CompositeObjectBase
    {
        // ids starting from 30
        public Garden(string id, string name, string displayName)
            : base(id, name, displayName)
        {
        }

    }

    [Serializable]
    public class GreenHouse : CompositeObjectBase, IControlUnit
    {
        private List<ICommand> _commands = new List<ICommand>() {
            new GenericCommand("opendoor",  "OpenDoor", "Открыть дверь"),
            new GenericCommand("closedoor", "CloseDoor", "Закрыть дверь")
        };

        // ids starting from 50
        public GreenHouse(string id, string name, string displayName)
            : base(id, name, displayName)
        {
        }

        public List<ICommand> AvailableCommands
        {
            get
            {
                return _commands;
            }
        }

        public Dictionary<string, string> Settings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void ExecuteCommand(ICommand command)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class Barrel : CompositeObjectBase, IControlUnit
    {

        private List<ICommand> _commands = new List<ICommand>() {
            new GenericCommand("fill",  "Fill", "Залить" ),
            new GenericCommand("empty", "Empty", "Слить" ),
            new GenericCommand("block", "Block", "Перекрыть" )
        };

        public Barrel(string id, string name, string displayName)
            : base(id, name, displayName)
        {
        }

        public List<ICommand> AvailableCommands
        {
            get
            {
                return _commands;
            }
        }

        public Dictionary<string, string> Settings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void ExecuteCommand(ICommand command)
        {
            throw new NotImplementedException();
        }

    }

    [Serializable]
    public class GenericCommand : GenericObjectBase, ICommand
    {

        public GenericCommand(string id, string name, string displayName)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
        }

        public bool CanExecute
        {
            get; set;
        }

        //public string DisplayName
        //{
        //    get; set;
        //}

        //public string Id
        //{
        //    get; set;
        //}

        //public string Name
        //{
        //    get; set;
        //}
    }

    #region Sensors

    [Serializable]
    public abstract class SensorBase : GenericObjectBase, ISensor
    {
        protected SensorBase()
        {
            
        }

        protected SensorBase(string id, string name, string displayName)             
            : base(id, name, displayName)
        {
            MeasureTime = DateTime.UtcNow.AddHours(3);
        }

        public string MeasureUnit { get; protected set; }
        public SensorType SensorType { get; protected set; }
        public string Value { get; set; }

        public DateTime MeasureTime { get; set; }
    }

    [Serializable]
    public class TemperatureSensor : SensorBase
    {
        private TemperatureSensor()
        { }

        public TemperatureSensor(string id, string name, string displayName) : base(id, name, displayName)
        {

            SensorType = SensorType.Temperature;
            MeasureUnit = "град";
        }

    }

    [Serializable]
    public class MovementSensor : SensorBase
    {
        public MovementSensor(string id, string name, string displayName) : base(id, name, displayName)
        {

            SensorType = SensorType.State;
            MeasureUnit = "";
        }
    }

    [Serializable]
    public class LevelSensor : SensorBase
    {
        public LevelSensor(string id, string name, string displayName) : base(id, name, displayName)
        {

            SensorType = SensorType.Level;
            MeasureUnit = "%";
        }

    }

    /// <summary>
    /// Датчик состояния
    /// </summary>
    [Serializable]
    public class StateSensor : SensorBase
    {
        public StateSensor(string id, string name, string displayName) : base(id, name, displayName)
        {

            SensorType = SensorType.State;
            MeasureUnit = "";
        }

    }

    #endregion

    #endregion

}

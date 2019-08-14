using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome.Model
{
    public abstract class GenericObjectBase
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

        public string DisplayName { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public CompositeObject Parent { get; set; }

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

    }
}

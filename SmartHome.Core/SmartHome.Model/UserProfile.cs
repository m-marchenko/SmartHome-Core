using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome.Model
{
    public class UserProfile
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public List<CompositeObject> Content { get; set; }
    }
}

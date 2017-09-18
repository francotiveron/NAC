using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public class NacProject : NacObjectWithChildren {
        [DataMember]
        private NacDatabase _database;
        public NacDatabase Database {
            get {
                if (_database == null) {
                    _database = new NacDatabase();
                    _database.Path = Path + @"//";
                    _database.Name = Name + @"//";
                }
                return _database;
            }
            set {
                _database = value;
            }
        }
        [DataMember]
        public bool ReadInputs { get; set; }
        [DataMember]
        public bool WriteOutputs { get; set; }
        [DataMember]
        public bool Run { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext ctx) { Database.Name = Name + "//"; }

        public override string Name { get { return base.Name; } set { base.Name = value; /*if (Database != null) Database.Name = value + "//";*/ } }

        public NacProject() { /*Database = new NacDatabase();*/ }

        //public IEnumerable<NacTask> Tasks { get { return Children.Cast<NacTask>(); } }
    }
}

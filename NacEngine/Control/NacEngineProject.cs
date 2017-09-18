using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common;
using System.Runtime.Serialization;

namespace Nac.Engine.Control {
    [DataContract]
    public class NacEngineProject : NacEngineObjectWithChildren {
        [DataMember]
        public new NacProject Base { get { return base.Base as NacProject; } set { base.Base = value; } }
        public NacEngineProject(NacProject project) : base(project) { }
        public IEnumerable<NacEngineTask> Tasks { get { return Children.Cast<NacEngineTask>(); } }

        private NacEngineRuntime _runtime;
        public NacEngineRuntime Runtime {
            get {
                if (_runtime == null) _runtime = new NacEngineRuntime(this);
                return _runtime;
            }
        }

        private NacEngineDatabase _database;
        public NacEngineDatabase Database {
            get {
                if (_database == null) _database = new NacEngineDatabase(Base.Database);
                return _database;
            }
        }

        public bool ReadInputs { get { return Base.ReadInputs; } }
        public bool Run { get { return Base.Run; } }
        public bool WriteOutputs { get { return Base.WriteOutputs; } }

        public void Execute() {
            Database.CycleBegin();
            Parallel.ForEach(Tasks, task => task.Execute());
            Database.CycleEnd();
        }
    }
}

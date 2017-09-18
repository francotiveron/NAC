using Nac.Common;
using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Engine.Control {
    [DataContract]
    public class NacEngineTask : NacEngineObjectWithChildren {
        [DataMember]
        public new NacTask Base { get { return base.Base as NacTask; } set { base.Base = value; } }
        public NacEngineTask(NacTask task) : base(task) { }
        public IEnumerable<NacEngineSection> Sections { get { return Children.Cast<NacEngineSection>();} }
        public TimeSpan CycleTime { get { return Base.CycleTime; } }
        public TimeSpan CycleCountdown { get { return Base.CycleCountdown; } set { Base.CycleCountdown = value; } }

        private DateTime _lastExec = default(DateTime);
        public void Execute() {
            DateTime now = DateTime.Now;
            var _countdown = CycleTime - (now - _lastExec);

            if (_countdown > TimeSpan.Zero) { CycleCountdown = _countdown; return; }

            CycleCountdown = TimeSpan.Zero;
            _lastExec = now;
            foreach (var section in Sections) section.Execute();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common.Control;
using Nac.Common;
using System.Runtime.Serialization;

namespace Nac.Engine.Control {
    [DataContract]
    internal class NacEngineBlockSeq : NacEngineBlock {
        [DataMember]
        public new NacBlockSeq Base { get { return base.Base as NacBlockSeq; } set { base.Base = value; } }
        public NacEngineBlockSeq(NacBlockSeq block) : base(block) { }

        protected HashSet<string> Prev { get { return Base.Prev; } }

        public virtual IEnumerable<string> OutputConnections { get { return Base.OutputConnections; } }
        public virtual IEnumerable<string> InputConnections { get { return Base.InputConnections; } }

        public bool ExecuteOnAny { get { return Base.ExecuteOnAny; } }

        public virtual bool CanScheduleNext(NacEngineBlockSeq nextBlock) {
            return Scheduled && Quality == NacExecutionQuality.Good;
        }

        protected override bool CanExecute() {
            if (!Project.Run) return false;

            if (Prev.Count == 0) return true;
             
            var prevs = Prev
            .Select(path => Engine[path])
            .Cast<NacEngineBlockSeq>();

            Func<NacEngineBlockSeq, bool> prevFiring = (prevBlock) => prevBlock.CanScheduleNext(this);

            var canExecute = ExecuteOnAny ? prevs.Any(prevFiring) : prevs.All(prevFiring);

            return canExecute;
        }

        public override bool Execute() {
            Scheduled = CanExecute();
            return base.Execute();
        }
    }
}

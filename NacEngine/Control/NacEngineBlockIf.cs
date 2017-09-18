using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common.Control;
using Nac.Common;
using ExpressionEvaluator;
using System.Runtime.Serialization;

namespace Nac.Engine.Control {
    [DataContract]
    internal class NacEngineBlockIf : NacEngineBlockSeq {
        [DataMember]
        public new NacBlockIf Base { get { return base.Base as NacBlockIf; } set { base.Base = value; } }
        public NacEngineBlockIf(NacBlockIf block) : base(block) { }

        public HashSet<string> NextTrue { get { return Base.NextTrue; } }

        public HashSet<string> NextFalse { get { return Base.NextFalse; } }

        protected override CompiledExpressionType ExpressionType { get { return CompiledExpressionType.Expression; }
        }

        public override bool CanScheduleNext(NacEngineBlockSeq nextBlock) {
            return
                base.CanScheduleNext(nextBlock)
                && 
                ((bool)Result ? NextTrue.Any(path => path == nextBlock.Path) : NextFalse.Any(path => path == nextBlock.Path));
        }
    }
}

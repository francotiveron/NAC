using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common.Control;
using Nac.Common;
using ExpressionEvaluator;
using static NacEngineGlobal;
using Qualities = Nac.Common.Control.NacExecutionQuality;
using static NacUtils;
using System.Runtime.Serialization;

namespace Nac.Engine.Control {
    [DataContract]
    internal class NacEngineBlockCall : NacEngineBlockSeq {
        [DataMember]
        public new NacBlockCall Base { get { return base.Base as NacBlockCall; } set { base.Base = value; } }
        public NacEngineBlockCall(NacBlockCall block) : base(block) { }

        private NacEngineSection subroutine;
        public override bool Execute() {
            if (Prev.Count > 0) Scheduled = CanExecute();
            if (Scheduled) {
                if (Code != subroutine?.Base.Name || Quality == Qualities.BadCompilation || Quality == Qualities.Unknown)
                    Succeeds(() => {
                            var engine = G.Engine as NacEngine;
                            var nacPath = (NacPath)Path;
                            var nacTask = engine[nacPath.TaskPath] as NacEngineTask;
                            var subPath = nacTask.Sections.FirstOrDefault(section => section.Name == Code)?.Path;
                            return engine[subPath];
                        }
                        , ref subroutine
                        , true
                    );

                if (subroutine == null || !subroutine.IsSubroutine) { Quality = Qualities.BadCompilation; }
                else {
                    subroutine.Call();
                    Quality = Qualities.Good;
                }
            }
            else Quality = Qualities.Unknown;

            return Quality == Qualities.Good;
        }
    }
}

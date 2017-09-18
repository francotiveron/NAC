using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common.Control;
using Nac.Common;
using ExpressionEvaluator;
using System.Runtime.Serialization;
using Qualities = Nac.Common.Control.NacExecutionQuality;

namespace Nac.Engine.Control {
    [DataContract]
    internal class NacEngineBlockTimer : NacEngineBlockSeq {
        [DataMember]
        public new NacBlockTimer Base { get { return base.Base as NacBlockTimer; } set { base.Base = value; } }
        public NacEngineBlockTimer(NacBlockTimer block) : base(block) { }

        public TimeSpan Interval { get { return Base.Interval; } set { Base.Interval = value; } }

        private bool _tick;
        public override bool CanScheduleNext(NacEngineBlockSeq nextBlock) { return _tick; }

        protected override bool CanExecute() { return Project.Run; }

        private DateTime _lastTick = default(DateTime);
        public override bool Execute() {
            DateTime now = DateTime.Now;
            if (CanExecute()) {
                var _countdown = Interval - (now - _lastTick);

                if (_countdown > TimeSpan.Zero) {
                    Countdown = _countdown;
                    _tick = false;
                } else {
                    Countdown = TimeSpan.Zero;
                    _lastTick = now;
                    _tick = true;
                }
            }
            else {
                _lastTick = now + Countdown - Interval;
                _tick = false;
            }

            bool b = base.Execute();
            Quality = Quality == Qualities.Empty ? Qualities.Good : Quality;
            return b;
        }

        public int Reset(bool tick = false) {
            _lastTick = DateTime.Now;
            _tick = tick;
            return 0;
        }
    }
}

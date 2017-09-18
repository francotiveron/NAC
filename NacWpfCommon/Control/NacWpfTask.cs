using Nac.Common.Control;
using System;

namespace Nac.Wpf.Common.Control {
    public class NacWpfTask : NacWpfObjectWithChildren {
        public NacWpfTask(NacTask nacTask) : base(nacTask) { }
        public new NacTask Base { get { return base.Base as NacTask; } }

        public TimeSpan CycleTime {
            get { return Base.CycleTime; }
            set { if (value != Base.CycleTime) { Base.CycleTime = value; OnNotifyPropertyChanged("CycleTime"); } }
        }

        public TimeSpan CycleCountdown {
            get { return Base.CycleCountdown; }
            set { if (value != Base.CycleCountdown) { Base.CycleCountdown = value; OnNotifyPropertyChanged("CycleCountdown"); } }
        }
    }
}

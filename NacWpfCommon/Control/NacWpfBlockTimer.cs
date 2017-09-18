using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common.Control;

namespace Nac.Wpf.Common.Control {
    public class NacWpfBlockTimer : NacWpfBlockSeq {
        public NacWpfBlockTimer(NacBlockTimer nacBlockIf) : base(nacBlockIf) {   }
        public new NacBlockTimer Base { get { return base.Base as NacBlockTimer; } }

        public TimeSpan Interval {
            get { return Base.Interval; }
            set { if (value != Base.Interval) { Base.Interval = value; OnNotifyPropertyChanged("Interval"); } }
        }
    }
}

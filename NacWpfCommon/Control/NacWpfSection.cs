using Nac.Common.Control;
using System.Collections.Generic;
using System.Linq;

namespace Nac.Wpf.Common.Control {
    public class NacWpfSection : NacWpfObjectWithChildren {
        public NacWpfSection(NacSection nacSection) : base(nacSection) { }
        public new NacSection Base { get { return base.Base as NacSection; } }

        public IEnumerable<NacWpfBlock> Blocks { get { return Children.Cast<NacWpfBlock>(); } }

        private bool _online;
        public bool Online {
            get { return _online; }
            set { if (value != _online) { _online = value; OnNotifyPropertyChanged("Online"); } }
        }

        public bool IsSubroutine {
            get { return Base.IsSubroutine; }
            set { if (value != Base.IsSubroutine) { Base.IsSubroutine = value; OnNotifyPropertyChanged("IsSubroutine"); } }
        }
    }
}

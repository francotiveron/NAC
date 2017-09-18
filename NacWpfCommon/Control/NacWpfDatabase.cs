using Nac.Common;
using Nac.Common.Control;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Nac.Wpf.Common.Control {
    public class NacWpfDatabase : NacWpfObjectWithChildren {
        public NacWpfDatabase(NacDatabase nacDatabase) : base(nacDatabase) { }

        public IEnumerable<NacWpfTag> Tags { get { return Children.Cast<NacWpfTag>(); } }

        private bool _online;
        public bool Online {
            get { return _online; }
            set { if (value != _online) { _online = value; OnNotifyPropertyChanged("Online"); } }
        }

    }
}



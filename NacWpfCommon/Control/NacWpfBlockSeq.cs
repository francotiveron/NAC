using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common;
using Nac.Common.Control;
using System.ComponentModel;

namespace Nac.Wpf.Common.Control {
    public class NacWpfBlockSeq : NacWpfBlock {
        public NacWpfBlockSeq(NacBlockSeq nacBlock) : base(nacBlock) { }

        public new NacBlockSeq Base { get { return base.Base as NacBlockSeq; } }

        public HashSet<string> Next { get { return Base.Next; } }
        public HashSet<string> Prev { get { return Base.Prev; } }

        public bool ExecuteOnAny {
            get { return Base.ExecuteOnAny; }
            set { if (value != Base.ExecuteOnAny) { Base.ExecuteOnAny = value; OnNotifyPropertyChanged("ExecuteOnAny"); } }
        }

    }
}

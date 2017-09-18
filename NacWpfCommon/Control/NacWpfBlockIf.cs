using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common.Control;

namespace Nac.Wpf.Common.Control {
    public class NacWpfBlockIf : NacWpfBlockSeq {
        public NacWpfBlockIf(NacBlockIf nacBlockIf) : base(nacBlockIf) {   }
        public new NacBlockIf Base { get { return base.Base as NacBlockIf; } }

        public HashSet<string> NextTrue { get { return Base.NextTrue; } }
        public HashSet<string> NextFalse { get { return Base.NextFalse; } }
    }
}

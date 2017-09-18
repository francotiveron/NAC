using Nac.Common.Control;
using Nac.Wpf.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wpf.SectionEditor.NetworkModel {
    class NacWpfBlockCallViewModel1 : NacWpfBlockSeqViewModel1 {
        public NacWpfBlockCallViewModel1(NacWpfBlockCall block) : base(block) { }
        public new NacWpfBlockCall Block { get { return base.Block as NacWpfBlockCall; } }
        public new NacBlockCall Base { get { return base.Base as NacBlockCall; } }
    }
}

using Nac.Common.Control;
using Nac.Wpf.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wpf.SectionEditor.NetworkModel {
    class NacWpfBlockFuzzyViewModel1 : NacWpfBlockSeqViewModel1 {
        public NacWpfBlockFuzzyViewModel1(NacWpfBlockFuzzy block) : base(block) {
        }
        public new NacWpfBlockFuzzy Block { get { return base.Block as NacWpfBlockFuzzy; } }
        public new NacBlockFuzzy Base { get { return base.Base as NacBlockFuzzy; } }
    }
}

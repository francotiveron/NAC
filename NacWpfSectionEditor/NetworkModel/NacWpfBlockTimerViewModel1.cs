using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Wpf.Common.Control;
using Nac.Common.Control;

namespace Nac.Wpf.SectionEditor.NetworkModel {
    class NacWpfBlockTimerViewModel1 : NacWpfBlockSeqViewModel1 {
        public NacWpfBlockTimerViewModel1(NacWpfBlockTimer block) : base(block) {
            InputConnectors.Clear();
        }

        public new NacWpfBlockTimer Block { get { return base.Block as NacWpfBlockTimer; } }
        public new NacBlockTimer Base { get { return base.Base as NacBlockTimer; } }
        public TimeSpan Interval { get { return Base.Interval; } }

    }
}

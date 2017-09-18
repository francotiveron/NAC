using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Wpf.Common.Control;
using Nac.Common.Control;

namespace Nac.Wpf.SectionEditor.NetworkModel {
    class NacWpfBlockIfViewModel1 : NacWpfBlockSeqViewModel1 {
        public NacWpfBlockIfViewModel1(NacWpfBlockIf block) : base(block) {
            OutputConnectors[0].Name = "True";
            OutputConnectors.Add(new NacWpfConnectorViewModel1("False"));
        }
        public new NacWpfBlockIf Block { get { return base.Block as NacWpfBlockIf; } }
        public new NacBlockIf Base { get { return base.Base as NacBlockIf; } }
        public HashSet<string> NextTrue { get { return Base.NextTrue; } }
        public HashSet<string> NextFalse { get { return Base.NextFalse; } }

        public override IEnumerable<Tuple<string, string, string>> GetDestinations() {
            foreach (var path in NextTrue) yield return new Tuple<string, string, string>("True", path, null);
            foreach (var path in NextFalse) yield return new Tuple<string, string, string>("False", path, null);
        }
    }
}

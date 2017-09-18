using Nac.Common.Control;
using Nac.Wpf.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wpf.SectionEditor.NetworkModel {
    public class NacWpfBlockSeqViewModel1 : NacWpfBlockViewModel1 {

        public NacWpfBlockSeqViewModel1(NacWpfBlockSeq block) : base(block) {
            InputConnectors.Add(new NacWpfConnectorViewModel1(null));
            OutputConnectors.Add(new NacWpfConnectorViewModel1(null));
        }
        public new NacWpfBlock Block { get { return base.Block as NacWpfBlockSeq; } }
        public new NacBlockSeq Base { get { return base.Base as NacBlockSeq; } }
        public HashSet<string> Next { get { return Base.Next; } }
        public HashSet<string> Prev { get { return Base.Prev; } }

        public virtual IEnumerable<Tuple<string, string, string>> GetDestinations() {
            foreach (var path in Next) yield return new Tuple<string, string, string>(null, path, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public class NacBlockIf : NacBlockSeq {
        [DataMember]
        private HashSet<string> _nextTrue;
        public HashSet<string> NextTrue { get { if (_nextTrue == null) _nextTrue = new HashSet<string>(); return _nextTrue; } set { _nextTrue = value; } }

        public HashSet<string> NextFalse { get { return Next; } set { Next = value; } }

        public override IEnumerable<string> OutputConnections { get { return NextTrue.Union(NextFalse); } }

        public override void ConnectNext(string path, string connector) {
            if (connector == "True") NextTrue.Add(path);
            if (connector == "False") NextFalse.Add(path);
        }
        public override void DisconnectNext(string path, string connector) {
            if (connector == "True") NextTrue.Remove(path);
            if (connector == "False") NextFalse.Remove(path);
        }
        //public override void Dispose(bool disposing) {
        //    if (disposing) {
        //        foreach (var path in NextTrue) (Engine[path] as NacBlockSeq)?.Prev.Remove(Path);
        //    }
        //    base.Dispose(disposing);
        //}
    }
}

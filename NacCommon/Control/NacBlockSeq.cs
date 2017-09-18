using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public class NacBlockSeq: NacBlock {
        [DataMember]
        private HashSet<string> _next;
        public HashSet<string> Next { get { if (_next == null) _next = new HashSet<string>(); return _next; } set { _next = value; } }
        [DataMember]
        private HashSet<string> _prev;
        public HashSet<string> Prev { get { if (_prev == null) _prev = new HashSet<string>(); return _prev; } set { _prev = value; } }
        [DataMember]
        public bool ExecuteOnAny { get; set; }

        public virtual IEnumerable<string> OutputConnections { get { return Next; } }
        public virtual IEnumerable<string> InputConnections { get { return Prev; } }
        public virtual IEnumerable<string> Connections { get { return InputConnections.Union(OutputConnections); } }

        public virtual void ConnectNext(string path, string connector) { Next.Add(path); }
        public virtual void ConnectPrev(string path, string connector) { Prev.Add(path); }
        public virtual void DisconnectNext(string path, string connector) { Next.Remove(path); }
        public virtual void DisconnectPrev(string path, string connector) { Prev.Remove(path); }

        //public override void Dispose(bool disposing) {
        //    if (disposing) {
        //        foreach (var path in Prev) (Engine[path] as NacBlockSeq)?.Next.Remove(Path);
        //        foreach (var path in Next) (Engine[path] as NacBlockSeq)?.Prev.Remove(Path);
        //    }
        //    base.Dispose(disposing);
        //}

        //public bool IsStart { get { return Prev.Count == 0; } }

    }
}

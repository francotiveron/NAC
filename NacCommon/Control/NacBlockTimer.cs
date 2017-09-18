using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public class NacBlockTimer: NacBlockSeq {
        [DataMember]
        public TimeSpan Interval { get; set; }
    }
}

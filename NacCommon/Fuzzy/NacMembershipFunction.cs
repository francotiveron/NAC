using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common.Fuzzy {
    [DataContract, Serializable]
    public class NacMembershipFunction {}
    [DataContract, Serializable]
    public class NacTrapezoidalMembershipFunction : NacMembershipFunction {
        [DataMember]
        public virtual float m1 { get; set; }
        [DataMember]
        public virtual float m2 { get; set; }
        [DataMember]
        public virtual float m3 { get; set; }
        [DataMember]
        public virtual float m4 { get; set; }
        [DataMember]
        public virtual float min { get; set; }
        [DataMember]
        public virtual float max { get; set; }
        [DataMember]
        public virtual int edge { get; set; } //-1 Left, 0 no edge, 1 Right

        public NacTrapezoidalMembershipFunction() {
            m1 = 10f;
            m2 = 30f;
            m3 = 50f;
            m4 = 90f;
            min = 0f;
            max = 1f;
            edge = 0;
        }
    }
}

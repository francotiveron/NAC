using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common.Fuzzy {
    [DataContract, Serializable]
    public class NacFuzzySet {
        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual NacMembershipFunction MembershipFunction { get; set; }

        public NacFuzzySet(NacMembershipFunction mf) : this(mf, Guid.NewGuid().ToString()) { }

        public NacFuzzySet(NacMembershipFunction mf, string name) {
            Name = name;
            MembershipFunction = mf;
        }
    }
}

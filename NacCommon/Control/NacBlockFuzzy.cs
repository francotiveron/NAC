using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public class NacBlockFuzzy : NacBlockSeq {
        [DataMember]
        private HashSet<string> _fuzzyRules;
        public HashSet<string> FuzzyRules { get { return _fuzzyRules; } set { _fuzzyRules = value;  } }

    }
}

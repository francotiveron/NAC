using Nac.Common;
using Nac.Common.Fuzzy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace Nac.Common.Control {
    [DataContract, Serializable]
    public enum NacTagScope {[EnumMember]Local, [EnumMember]Input, [EnumMember]Output, [EnumMember]InOut };
    [DataContract, Serializable]
    public class NacTag : NacObject {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public NacTagScope Scope { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string Formula { get; set; }

        [DataMember]
        public NacTagValue Tag { get; set; }
        public double Value { get { return Tag.Value; } set { Tag = new NacTagValue(Tag.Quality, Tag.Timestamp, value); } }
        public NacValueQuality Quality { get { return Tag.Quality; } set { Tag = new NacTagValue(value, Tag.Timestamp, Tag.Value); } }
        public DateTime Timestamp { get { return Tag.Timestamp; } set { Tag = new NacTagValue(Tag.Quality, value, Tag.Value); } }

        [DataMember]
        private HashSet<NacFuzzySet> _fuzzySets;
        public HashSet<NacFuzzySet> FuzzySets { get { return _fuzzySets; } set { _fuzzySets = value; } }

        [DataMember]
        public TimeSpan History { get; set; }

        public override string ToString() {
            return Name;
        }
    }
}

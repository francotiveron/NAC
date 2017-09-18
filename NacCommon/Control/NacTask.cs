using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public class NacTask : NacObjectWithChildren {
        [DataMember]
        public TimeSpan CycleTime { get; set; }

        [DataMember]
        public TimeSpan CycleCountdown { get; set; }

    }
}

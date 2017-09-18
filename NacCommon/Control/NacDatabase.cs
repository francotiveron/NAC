using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Runtime.Serialization;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public class NacDatabase : NacObjectWithChildren {
        //public IEnumerable<NacTag> Tags { get { return Children.Cast<NacTag>(); } }
    }
}

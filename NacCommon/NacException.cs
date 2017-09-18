using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common {
    [DataContract, Serializable]
    public class NacException : Exception {
        public NacException(string message) : base(message) {}
    }
}

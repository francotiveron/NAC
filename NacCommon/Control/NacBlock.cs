using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public class NacBlock : NacObject {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public Point Position { get; set; }

        [DataMember]
        public NacExecutionStatus Status { get; set; }

        public NacExecutionQuality Quality { get { return Status.Quality; } set { Status = new NacExecutionStatus(Status.Scheduled, value, Status.Countdown); } }
        public bool Scheduled { get { return Status.Scheduled; } set { Status = new NacExecutionStatus(value, Status.Quality, Status.Countdown); } }
        public TimeSpan Countdown { get { return Status.Countdown; } set { Status = new NacExecutionStatus(Status.Scheduled, Status.Quality, value); } }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public enum NacExecutionQuality {[EnumMember]Unknown, [EnumMember]Empty, [EnumMember]BadCompilation, [EnumMember]BadExecution, [EnumMember]Good }
    [DataContract, Serializable]
    public struct NacExecutionStatus {
        [DataMember]
        public bool Scheduled { get; set; }
        [DataMember]
        public NacExecutionQuality Quality { get; set; }
        [DataMember]
        public TimeSpan Countdown { get; set; }

        public static implicit operator NacExecutionQuality(NacExecutionStatus v) { return v.Quality; }
        public static implicit operator NacExecutionStatus(NacExecutionQuality q) { return new NacExecutionStatus(false, q); }

        public NacExecutionStatus(
            bool scheduled = false
            , NacExecutionQuality quality = NacExecutionQuality.Unknown
            , TimeSpan countdown = default(TimeSpan)
        ) {
            Scheduled = scheduled;
            Quality = quality;
            Countdown = countdown;
        }
        public static bool operator ==(NacExecutionStatus v1, NacExecutionStatus v2) {
            return v1.Scheduled == v2.Scheduled && v1.Quality == v2.Quality && v1.Countdown == v2.Countdown;
        }
        public static bool operator !=(NacExecutionStatus v1, NacExecutionStatus v2) {
            return !(v1 == v2);
        }
        public override bool Equals(object obj) {
            return this == (NacExecutionStatus)obj;
        }

        public override int GetHashCode() {
            return Quality.GetHashCode() + 17 * Scheduled.GetHashCode() + 13 * Countdown.GetHashCode();
        }

        public override string ToString() {
            return $"({(Scheduled ? "Scheduled":"Not Scheduled")}, {Quality}, {Countdown})";
        }
    }
}

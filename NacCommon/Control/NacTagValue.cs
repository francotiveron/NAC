using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using static NacUtils;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public enum NacValueQuality {[EnumMember]Unknown, [EnumMember]Good, [EnumMember]Bad, [EnumMember]Uncertain }
    [DataContract, Serializable]
    public partial struct NacTagValue {
        [DataMember]
        public NacValueQuality Quality { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }

        public NacTagValue(NacValueQuality quality, DateTime timestamp, object value) {
            Quality = quality;
            Timestamp = timestamp;
            Type type = value.GetType();
            //Value = Cast(value, (Value = 0.0d).GetType());
            Value = (double)value;
        }

        public NacTagValue(NacValueQuality quality, object value) : this(quality, DateTime.Now, value) { }

        //public NacTagValue(dynamic dyn) {
        public NacTagValue(object dyn) {
            if (dyn is NacTagValue) {
                var tagValue = (NacTagValue)dyn;
                Quality = tagValue.Quality;
                Timestamp = tagValue.Timestamp;
                Value = tagValue.Value;
            } 
            else if (dyn is bool) {
                bool b = (bool)dyn;
                Value = b ? 1 : 0;
                Quality = NacValueQuality.Good;
                Timestamp = DateTime.Now;
            } else {
                double v = default(double);
                if (Succeeds(() => { v = (double)dyn; })) {
                    Value = v;
                    Quality = NacValueQuality.Good;
                    Timestamp = DateTime.Now;
                } else {
                    Quality = NacValueQuality.Uncertain;
                    Timestamp = DateTime.Now;
                    Value = 0.0;
                }
            }



            //if ((((object)dyn).GetType()).IsCastableTo(typeof(double))) {
            //    Value = (double)dyn;
            //    Quality = NacValueQuality.Good;
            //    Timestamp = DateTime.Now;
            //}
            //else {
            //    Quality = NacValueQuality.Uncertain;
            //    Timestamp = DateTime.Now;
            //    Value = 0.0;
            //}
        }

        [DataMember]
        public double Value { get; set; }

        public override string ToString() {
            return $"({Value}, {Quality}, {Timestamp})";
        }

        public static implicit operator double(NacTagValue v) { return v.Value; }
        public static implicit operator NacTagValue(double d) { return new NacTagValue(d); }
        public static implicit operator float(NacTagValue v) { return (float)v.Value; }
        public static implicit operator NacTagValue(float f) { return new NacTagValue(f); }

        public override bool Equals(object obj) {
            return this == (NacTagValue)obj;
        }

        public override int GetHashCode() {
            return Value.GetHashCode() + 17 * Quality.GetHashCode() + 11 * Timestamp.GetHashCode();
        }

        public static bool operator ==(NacTagValue v1, NacTagValue v2) {
            return (v1.Value == v2.Value || (double.IsNaN(v1.Value) && double.IsNaN(v2.Value))) && v1.Quality == v2.Quality && v1.Timestamp == v2.Timestamp;
        }

        public static bool operator !=(NacTagValue v1, NacTagValue v2) {
            return !(v1 == v2);
        }

    }
    //operators
    public partial struct NacTagValue {
        
        public static NacTagValue operator +(NacTagValue v1, NacTagValue v2) {
            var value = v1.Value + v2.Value;
            var quality = v1.Quality == v2.Quality ? v1.Quality : NacValueQuality.Uncertain;
            var timestamp = DateTime.Now;

            return new NacTagValue(quality, timestamp, value);
        }

        public static NacTagValue operator +(NacTagValue v, /*dynamic*/ object nbr) {
            double value = default(double);
            NacValueQuality quality = default(NacValueQuality);

            if (!Succeeds(() => {
                value = v.Value + (double)nbr;
                quality = v.Quality;
            })) {
                value = double.NaN;
                quality = NacValueQuality.Bad;
            }

            return new NacTagValue(quality, DateTime.Now, value);
        }

        public static NacTagValue operator +(/*dynamic*/ double nbr, NacTagValue v) {
            return v + nbr;
        }

        public static NacTagValue operator -(NacTagValue v) {
            return new NacTagValue(v.Quality, v.Timestamp, -v.Value);
        }

        public static NacTagValue operator -(NacTagValue v1, NacTagValue v2) {
            return v1 + (-v2);
        }

        public static NacTagValue operator -(NacTagValue v, /*dynamic*/ double nbr) {
            return v + (-nbr);
        }

        public static NacTagValue operator -(/*dynamic*/ double nbr, NacTagValue v) {
            return nbr + (-v);
        }

        public static NacTagValue operator *(NacTagValue v1, NacTagValue v2) {
            var value = v1.Value * v2.Value;
            var quality = v1.Quality == v2.Quality ? v1.Quality : NacValueQuality.Uncertain;
            var timestamp = DateTime.Now;

            return new NacTagValue(quality, timestamp, value);
        }

        public static NacTagValue operator *(NacTagValue v, /*dynamic*/ double nbr) {
            double value = default(double);
            NacValueQuality quality = default(NacValueQuality);

            if (!Succeeds(() => {
                value = v.Value * nbr;
                quality = v.Quality;
            })) {
                value = double.NaN;
                quality = NacValueQuality.Bad;
            }

            return new NacTagValue(quality, DateTime.Now, value);
        }

        public static NacTagValue operator *(/*dynamic*/ double nbr, NacTagValue v) {
            return v * nbr;
        }

        public static NacTagValue operator /(NacTagValue v1, NacTagValue v2) {
            double value = default(double);
            NacValueQuality quality = default(NacValueQuality);

            if (v2.Value != 0.0) {
                value = v1.Value / v2.Value;
                quality = v1.Quality == v2.Quality ? v1.Quality : NacValueQuality.Uncertain;
            } else {
                value = double.NaN;
                quality = NacValueQuality.Bad;
            }

            return new NacTagValue(quality, DateTime.Now, value);
        }

        public static NacTagValue operator /(NacTagValue v, /*dynamic*/ double nbr) {
            double value = default(double);
            NacValueQuality quality = default(NacValueQuality);

            if (nbr != 0.0 && !Succeeds(() => {
                value = v.Value / nbr;
                quality = v.Quality;
            })) {
                value = double.NaN;
                quality = NacValueQuality.Bad;
            }

            return new NacTagValue(quality, DateTime.Now, value);
        }

        public static NacTagValue operator /(/*dynamic*/ double nbr, NacTagValue v) {
            double value = default(double);
            NacValueQuality quality = default(NacValueQuality);

            if (v.Value != 0.0 && !Succeeds(() => {
                value = nbr / v.Value;
                quality = v.Quality;
            })) {
                value = double.NaN;
                quality = NacValueQuality.Bad;
            }

            return new NacTagValue(quality, DateTime.Now, value);
        }
        public static NacTagValue operator %(NacTagValue v1, NacTagValue v2) {
            double value = default(double);
            NacValueQuality quality = default(NacValueQuality);

            if (v2.Value != 0.0 && !Succeeds(() => {
                value = v1.Value % v2.Value;
                quality = v1.Quality;
            })) {
                value = double.NaN;
                quality = NacValueQuality.Bad;
            }

            return new NacTagValue(quality, DateTime.Now, value);
        }

        public static NacTagValue operator %(/*dynamic*/ double nbr, NacTagValue v) {
            double value = default(double);
            NacValueQuality quality = default(NacValueQuality);

            if (v.Value != 0.0 && !Succeeds(() => {
                value = nbr % v.Value;
                quality = v.Quality;
            })) {
                value = double.NaN;
                quality = NacValueQuality.Bad;
            }

            return new NacTagValue(quality, DateTime.Now, value);
        }

        public static NacTagValue operator %(NacTagValue v, /*dynamic*/ double nbr) {
            double value = default(double);
            NacValueQuality quality = default(NacValueQuality);

            if (nbr != 0.0 && !Succeeds(() => {
                value = v.Value % nbr;
                quality = v.Quality;
            })) {
                value = double.NaN;
                quality = NacValueQuality.Bad;
            }

            return new NacTagValue(quality, DateTime.Now, value);
        }

        public static bool operator >(NacTagValue v1, /*dynamic*/ object nbr) {
            return v1.Value > (double)nbr;
        }
        public static bool operator <(NacTagValue v1, /*dynamic*/ object nbr) {
            return v1.Value < (double)nbr;
        }

        public static bool operator >(/*dynamic*/ double nbr, NacTagValue v1) {
            return nbr > v1.Value;
        }
        public static bool operator <(/*dynamic*/ double nbr, NacTagValue v1) {
            return nbr < v1.Value;
        }

        public static bool operator >(NacTagValue v1, NacTagValue v2) {
            return v1.Value > v2.Value;
        }

        public static bool operator <(NacTagValue v1, NacTagValue v2) {
            return v1.Value < v2.Value;
        }


        public static bool operator >=(NacTagValue v1, /*dynamic*/ double nbr) {
            return v1.Value >= nbr;
        }
        public static bool operator <=(NacTagValue v1, /*dynamic*/ double nbr) {
            return v1.Value <= nbr;
        }

        public static bool operator >=(/*dynamic*/ double nbr, NacTagValue v1) {
            return nbr >= v1.Value;
        }
        public static bool operator <=(/*dynamic*/ double nbr, NacTagValue v1) {
            return nbr <= v1.Value;
        }

        public static bool operator >=(NacTagValue v1, NacTagValue v2) {
            return v1.Value >= v2.Value;
        }

        public static bool operator <=(NacTagValue v1, NacTagValue v2) {
            return v1.Value <= v2.Value;
        }

    }
}

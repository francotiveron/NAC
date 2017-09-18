using ExpressionEvaluator;
using Nac.Common;
using Nac.Common.Control;
using Nac.Common.Fuzzy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Runtime.Serialization;
using System.Dynamic;
using System.Reflection;

namespace Nac.Engine.Control {
    [DataContract]
    [DynamicLinqType]
    public partial class NacEngineTag : NacEngineObject {
        [DataMember]
        public new NacTag Base { get { return base.Base as NacTag; } set { base.Base = value; } }
        public NacEngineTag(NacTag tag) : base(tag) { }

        public NacTagScope Scope { get { return Base.Scope; } }
        public string Address { get { return Base.Address; } }
        public NacTagValue ToWrite { get; set; }
        public NacTagValue Tag {
            get { return Base.Tag; }
            set {
                if (value != Base.Tag) {
                    ToWrite = value;
                    if (
                        value.Value != Base.Value 
                        && (
                            Scope == NacTagScope.Output
                            || Scope == NacTagScope.InOut
                            )
                    ) WriteOutput = true;
                    else WriteComplete();
                }
            }
        }
        public void WriteComplete() {
            Base.Tag = new NacTagValue(NacValueQuality.Good, ToWrite.Value);
            WriteOutput = false;
        }
        public void Update(NacTagValue value) {
            Base.Tag = value;
        }
        public double Value {
            get { return Base.Value; }
            set { if (WriteOutput = value != Base.Value) Base.Value = value; }
        }
        public NacValueQuality Quality { get { return Base.Quality; } set { Base.Quality = value; } }
        public DateTime Timestamp { get { return Base.Timestamp; } set { Base.Timestamp = value; } }

        private bool _writeOutput;
        public bool WriteOutput {
            get { return _writeOutput; }
            set {
                _writeOutput = value;
                if (value && Quality == NacValueQuality.Good) HistoryAdd(Value);
            }
        }

        public HashSet<NacFuzzySet> FuzzySets { get { return Base.FuzzySets; } }
        public TimeSpan History { get { return Base.History; } }

        public override void OnBaseChanged(string property = null) {
            base.OnBaseChanged(property);
            if (property == "FuzzySets") LastFuzzyChange = DateTime.Now;
            if (property == "Value") { ToWrite = Base.Value; WriteOutput = true; }
        }
        public DateTime LastFuzzyChange { get; set; }


        //public static implicit operator double(NacEngineTag tag) { return tag.Tag.Value; }
        public static explicit operator double(NacEngineTag tag) { return tag.Tag.Value; }
        //public static explicit operator NacTagValue(NacEngineTag tag) { return tag.Tag; }
        //public static implicit operator NacTagValue(double d) { return new NacTagValue(NacValueQuality.Unknown, DateTime.Now, d); }

        public override bool Equals(object obj) {
            return base.Equals(obj);
            //try { return Tag == (NacTagValue)obj; } catch { return false; }
        }

        public override int GetHashCode() {
            return Tag.GetHashCode();
        }
    }
    //Dynamic
    public partial class NacEngineTag {
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            switch (binder.Name) {
                case "Avg":
                case "Min":
                case "Max":
                case "Sum":
                    return InvokeDynamicAggregate(binder.Name, args, out result);
                default: break;
            }

            result = null; return false;
        }
    }

    //Operators
    public partial class NacEngineTag {

        public static bool operator ==(NacEngineTag t1, NacEngineTag t2) {
            return t1?.Tag == t2?.Tag;
        }

        public static bool operator !=(NacEngineTag t1, NacEngineTag t2) {
            return !(t1 == t2);
        }

        public static NacTagValue operator +(NacEngineTag t1, NacEngineTag t2) {
            return t1.Tag + t2.Tag;
        }

        public static NacTagValue operator +(NacEngineTag t, /*dynamic*/ double nbr) {
            return t.Tag + nbr;
        }

        public static NacTagValue operator +(/*dynamic*/ double nbr, NacEngineTag t) {
            return t + nbr;
        }

        public static NacTagValue operator -(NacEngineTag t) {
            return -t.Tag;
        }

        public static NacTagValue operator -(NacEngineTag t1, NacEngineTag t2) {
            return t1.Tag - t2.Tag;
        }

        public static NacTagValue operator -(NacEngineTag t, /*dynamic*/ double nbr) {
            return t + (-nbr);
        }

        public static NacTagValue operator -(/*dynamic*/ double nbr, NacEngineTag t) {
            return nbr + (-t);
        }

        public static NacTagValue operator *(NacEngineTag t1, NacEngineTag t2) {
            return t1.Tag * t2.Tag;
        }

        public static NacTagValue operator *(NacEngineTag t, /*dynamic*/ double nbr) {
            return t.Tag * nbr;
        }

        public static NacTagValue operator *(/*dynamic*/ double nbr, NacEngineTag t) {
            return t.Tag * nbr;
        }

        public static NacTagValue operator /(NacEngineTag t1, NacEngineTag t2) {
            return t1.Tag / t2.Tag;
        }

        public static NacTagValue operator /(NacEngineTag t, /*dynamic*/ double nbr) {
            return t.Tag / nbr;
        }

        public static NacTagValue operator /(/*dynamic*/ double nbr, NacEngineTag t) {
            return nbr / t.Tag;
        }

        public static NacTagValue operator %(NacEngineTag t1, NacEngineTag t2) {
            return t1.Tag % t2.Tag;
        }

        public static NacTagValue operator %(NacEngineTag t, /*dynamic*/ double nbr) {
            return t.Tag % nbr;
        }

        public static NacTagValue operator %(/*dynamic*/ double nbr, NacEngineTag t) {
            return nbr % t.Tag;
        }

        public static bool operator >(NacEngineTag t1, NacEngineTag t2) {
            return t1.Tag > t2.Tag;
        }
        public static bool operator <(NacEngineTag t1, NacEngineTag t2) {
            return t1.Tag < t2.Tag;
        }
        public static bool operator >(NacEngineTag t1, /*dynamic*/ double nbr) {
            return t1.Tag > nbr;
        }
        public static bool operator <(NacEngineTag t1, /*dynamic*/ double nbr) {
            return t1.Tag < nbr;
        }
        public static bool operator >(double nbr, NacEngineTag t2) {
            return nbr > t2.Tag;
        }
        public static bool operator <(double nbr, NacEngineTag t2) {
            return nbr < t2.Tag;
        }

        public static bool operator >=(NacEngineTag t1, NacEngineTag t2) {
            return t1.Tag >= t2.Tag;
        }
        public static bool operator <=(NacEngineTag t1, NacEngineTag t2) {
            return t1.Tag <= t2.Tag;
        }
        public static bool operator >=(NacEngineTag t1, /*dynamic*/ double nbr) {
            return t1.Tag >= nbr;
        }
        public static bool operator <=(NacEngineTag t1, /*dynamic*/ double nbr) {
            return t1.Tag <= nbr;
        }
        public static bool operator >=(double nbr, NacEngineTag t2) {
            return nbr >= t2.Tag;
        }
        public static bool operator <=(double nbr, NacEngineTag t2) {
            return nbr <= t2.Tag;
        }
    }

    //History
    public partial class NacEngineTag {
        public struct Sample {
            public DateTime Timestamp { get; set; }
            public float Value { get; set; }
            public bool IsEarlierThan(DateTime dt) { return Timestamp <= dt; }
            public bool IsLaterThan(DateTime dt) { return Timestamp >= dt; }
            public bool IsBetween(DateTime begin, DateTime end) { return IsLaterThan(begin) && IsEarlierThan(end); }
        }
        [DataMember]
        private List<Sample> _samples;
        public List<Sample> Samples { get { if (_samples == null) _samples = new List<Sample>(); return _samples; } }

        public void RemoveOldSamples() {
            var now = DateTime.Now;
            var earliestTime = now - History;
            var oldSamples = Samples.Where(sample => sample.Timestamp < earliestTime).ToArray();
            foreach (var sample in oldSamples) Samples.Remove(sample);
        }

        public void HistoryAdd(double Value) {
            if (double.IsNaN(Value) || double.IsInfinity(Value)) return;

            var now = DateTime.Now;
            if (Samples.Count == 0) { Samples.Add(new Sample { Timestamp = now, Value = (float)Value }); return; }

            var minSampleGap = TimeSpan.FromMilliseconds(History.TotalMilliseconds / 1000d);
            if ((now - Samples.Last().Timestamp) < minSampleGap) return;

            if (Samples.Count == 0 || Value != Samples.Last().Value) Samples.Add(new Sample { Timestamp = now, Value = (float)Value });
        }

        public double this[DateTime dt] { get { return Get(dt); } }

        public double this[TimeSpan ts] { get { return Get(DateTime.Now - ts); } }

        public double this[string DTorTS] {
            get {
                if (string.IsNullOrEmpty(DTorTS)) return Value;

                var dateTime = DateTime.Now;

                if (DTorTS[0] == '+' || DTorTS[0] == '-') {
                    var sTimeSpan = DTorTS[0] == '+' ? DTorTS.Remove(0, 1) : DTorTS;
                    var timeSpan = NacConvert.To<TimeSpan>(sTimeSpan);
                    dateTime = DateTime.Now + timeSpan;
                } else dateTime = NacConvert.To<DateTime>(DTorTS);

                return Get(dateTime);
            }
        }

        public double Get(DateTime dt, bool interpolated = false) {
            if (Samples.Count == 0) return Value;
            if (dt <= Samples.First().Timestamp) return Samples.First().Value;
            if (dt >= Samples.Last().Timestamp) return Samples.Last().Value;
            var sampleBefore = Samples.Last(sample => sample.Timestamp <= dt);
            if (!interpolated) return sampleBefore.Value;

            var sampleAfter = Samples.First(sample => sample.Timestamp >= dt);
            var timeGap = sampleAfter.Timestamp - sampleBefore.Timestamp;
            if (timeGap.Equals(TimeSpan.Zero)) return sampleBefore.Value;
            var range = sampleAfter.Value - sampleBefore.Value;
            var ratio = (dt - sampleBefore.Timestamp).TotalMilliseconds / timeGap.TotalMilliseconds;
            var result = sampleBefore.Value + range * ratio;
            return result;
        }
    }

    //Aggregate
    public partial class NacEngineTag {
        private Dictionary<string, MethodInfo> _aggregateMethods;
        private Dictionary<string, MethodInfo> AggregateMethods {
            get {
                if (_aggregateMethods == null) {
                    var aggregates = new string[] { "Average", "Min", "Max", "Sum" };

                    Func<MethodInfo, bool> aggregateSelector = method => {
                        var pars = method.GetParameters();
                        if (pars.Count() != 2) return false;
                        var genPars = pars[1].ParameterType.GetGenericArguments();
                        if (genPars.Count() != 2) return false;
                        if (genPars[1].Equals(typeof(float))) return true;
                        return false;
                    };

                    _aggregateMethods =
                        typeof(Enumerable)
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Where(m => aggregates.Contains(m.Name))
                        .Where(m => aggregateSelector(m))
                        .Select(m => m.MakeGenericMethod(typeof(Sample)))
                        .ToDictionary(m => m.Name);

                    _aggregateMethods["Avg"] = _aggregateMethods["Average"];
                }
                return _aggregateMethods;
            }
        }


        private bool InvokeDynamicAggregate(string name, object[] args, out object result) {
            IEnumerable<Sample> samples;
            MethodInfo method = AggregateMethods[name];
            Func<Sample, float> valueSelector = sample => sample.Value;

            DateTime now = DateTime.Now, end = now, begin = end - History;
            string filter = "true";
            object[] filterPars = null;

            if (args.Length > 0) begin = end - NacConvert.To<TimeSpan>(args[0]);
            if (args.Length > 1) filter = (string)args[1];
            if (args.Length > 2) filterPars = args.Skip(2).Select(arg => arg is NacEngineTag ? ((NacEngineTag)arg).Value : arg).ToArray();

            samples =
                Samples
                .Where(sample => sample.IsBetween(begin, end))
                .Where(sample => !float.IsNaN(sample.Value) && !float.IsInfinity(sample.Value))
                .AsQueryable()
                .Where(filter, filterPars);

            if (samples.Count() > 0) {
                object[] pars = { samples, valueSelector };
                result = method.Invoke(null, pars);
            } else result = Value;

            return result != null;
        }

    }
}

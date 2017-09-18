using Nac.Common;
using Nac.Common.Control;
using Nac.Wcf.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static NacEngineGlobal;

namespace Nac.Engine.Control {
    [DataContract]
    public class NacEngineDatabase : NacEngineObjectWithChildren {
        [DataMember]
        public new NacDatabase Base { get { return base.Base as NacDatabase; } set { base.Base = value; } }
        public NacEngineDatabase(NacDatabase database) : base(database) { }

        public IEnumerable<NacEngineTag> Tags { get { return Children.Cast<NacEngineTag>(); } }
        //public string Path { get { return Base.Path; } set { Base.Path = value; } }
        public NacEngineField Field { get { return G.Field as NacEngineField; } }

        public NacEngineTag this[string name] {
            get { return Tags.FirstOrDefault(tag => tag.Name == name); }
        }

        public void CycleBegin() { if (Project.ReadInputs) ReadInputs(); }

        public void CycleEnd() {
            if (Project.WriteOutputs) WriteOutputs();
            foreach (var tag in Tags) tag.RemoveOldSamples();
        }

        private void SetQualities(NacEngineTag[] tags, NacValueQuality quality) {
            for (int i = 0; i < tags.Length; i++)
                tags[i].Quality = quality;
        }
        public void ReadInputs() {
            NacEngineTag[] inputTags = null;
            try {
                inputTags = Tags.Where(t => t.Scope == NacTagScope.Input || (t.Scope == NacTagScope.InOut && !t.WriteOutput)).ToArray();
                if (inputTags.Length == 0) return;

                NacTagValue[] values = Field.Read(inputTags.Select(it => it.Address).ToArray());
                if (values != null) {
                    for (int i = 0; i < values.Length; i++)
                        inputTags[i].Update(values[i]);
                } else SetQualities(inputTags, NacValueQuality.Bad);
            } catch (Exception x) when (G.Log(x)) {
                SetQualities(inputTags, NacValueQuality.Unknown);
            }
        }
        public void WriteOutputs() {
            NacEngineTag[] outputTags = null;
            try {
                outputTags = Tags.Where(t => t.Scope == NacTagScope.Output || (t.Scope == NacTagScope.InOut && t.WriteOutput)).ToArray();
                if (outputTags.Length == 0) return;

                bool[] successes = Field.Write(outputTags.Select(ot => ot.Address).ToArray(), outputTags.Select(ot => ot.ToWrite).ToArray());

                if (successes != null) {
                    for (int i = 0; i < successes.Length; i++)
                        if (successes[i]) { outputTags[i].WriteComplete(); }
                        else outputTags[i].Quality = NacValueQuality.Bad;
                } else SetQualities(outputTags, NacValueQuality.Bad);
            } catch (Exception x) when (G.Log(x)) {
                SetQualities(outputTags, NacValueQuality.Unknown);
            }
        }
        //public void WriteOutputs() {
        //    try {
        //        var outputTags = Tags.Where(t => t.Scope == NacTagScope.Output || t.Scope == NacTagScope.InOut);
        //        foreach (var tag in outputTags) {
        //            if (tag.WriteOutput && !Field.Write(tag.Address, tag.Tag)) tag.Quality = NacValueQuality.Bad;
        //            tag.WriteOutput = false;
        //        }
        //    } catch (Exception x) when (G.Log(x)) { }
        //}

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            try {
                result = Tags.First(x => x.Name == binder.Name);
                return true;
            } catch (Exception x) when (G.Log(x, binder)) { result = null; }
            return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value) {
            try {
                var tag = Tags.First(x => x.Name == binder.Name);
                tag.Tag = new NacTagValue(value);
                return true;
            } catch (Exception x) when (G.Log(x, binder)) { }
            return false;
        }

    }
}

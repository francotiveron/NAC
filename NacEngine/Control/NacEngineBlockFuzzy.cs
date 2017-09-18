using Accord.Fuzzy;
using Nac.Common;
using Nac.Common.Control;
using Nac.Fuzzy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Nac.Fuzzy.Common.NacFuzzyRule;
using static NacEngineGlobal;
using Qualities = Nac.Common.Control.NacExecutionQuality;

namespace Nac.Engine.Control {
    [DataContract]
    internal class NacEngineBlockFuzzy : NacEngineBlockSeq {
        [DataMember]
        public new NacBlockFuzzy Base { get { return base.Base as NacBlockFuzzy; } set { base.Base = value; } }
        public NacEngineBlockFuzzy(NacBlockFuzzy block) : base(block) { }
        private HashSet<NacEngineTag> _inputTags;
        private HashSet<NacEngineTag> _outputTags;
        private InferenceSystem _is;

        public override void OnBaseChanged(string property = null) {
            base.OnBaseChanged(property);
            if (property == "FuzzyRules") LastFuzzyChange = DateTime.Now;
        }
        public DateTime LastFuzzyChange { get; set; }

        private bool CompilationNeeded {
            get {
                return
                    LastFuzzyChange >= LastCompileTime
                    || _inputTags.Any(tag => tag.LastFuzzyChange >= LastCompileTime)
                    || _outputTags.Any(tag => tag.LastFuzzyChange >= LastCompileTime)
                    ;
            }
        }

        public override bool Execute() {
            Scheduled = CanExecute();

            if (!Scheduled) {
                Quality = Qualities.Unknown;
                return true;
            }

            if (CompilationNeeded)
                try { Compile(); }
                catch (Exception x) when (G.Log(x)) { Quality = Qualities.BadCompilation; return false; }

            try {
                foreach (var i in _inputTags) _is.SetInput(i.Name, (float)i.Value);
                foreach (var o in _outputTags)
                    try { o.Tag = new NacTagValue((double)_is.Evaluate(o.Name)); Quality = Qualities.Good; }
                    catch (Exception x) when (G.Log(x, o)) { o.Quality = NacValueQuality.Uncertain; Quality = Qualities.BadExecution; }
            }
            catch { Quality = Qualities.BadExecution; }
            return Quality == Qualities.Good;
        }

        public DateTime LastCompileTime { get; set; }
        protected virtual void Compile() {
            _inputTags = new HashSet<NacEngineTag>();
            _outputTags = new HashSet<NacEngineTag>();
            var fuzzyRules = Base.FuzzyRules.Select(fr => new NacFuzzyRule(fr));
            var fuzzyInputs = fuzzyRules.SelectMany(fr => fr.FuzzyInputs);
            var fuzzyOutputs = fuzzyRules.Select(fr => fr.FuzzyOutput);

            var linguisticVariables = new Dictionary<string, LinguisticVariable>();
            foreach (var fuzzyClause in fuzzyInputs) if (!linguisticVariables.ContainsKey(fuzzyClause?.LinVar)) ParseClause(fuzzyClause, linguisticVariables, _inputTags);
            foreach (var fuzzyClause in fuzzyOutputs) if (!linguisticVariables.ContainsKey(fuzzyClause?.LinVar)) ParseClause(fuzzyClause, linguisticVariables, _outputTags);
            var fuzzyDB = new Database();
            foreach (var lv in linguisticVariables.Values) fuzzyDB.AddVariable(lv);
            _is = new InferenceSystem(fuzzyDB, new CentroidDefuzzifier(1000));
            foreach (var fr in fuzzyRules) _is.NewRule(Guid.NewGuid().ToString(), fr.Content);
            LastCompileTime = DateTime.Now;
        }

        private void ParseClause(FuzzyClause? fuzzyClause, Dictionary<string, LinguisticVariable> lvDict, HashSet<NacEngineTag> tagSet) {
            var tag = Database[fuzzyClause?.LinVar];
            if (tag == null) throw new NacException($"Tag {fuzzyClause?.LinVar} Not Found in RTDB");
            if (tag.FuzzySets == null) throw new NacException($"Tag {tag.Name} is not Fuzzy");
            if (tag.FuzzySets.All(fuzzySet => !fuzzySet.Name.Equals(fuzzyClause?.FuzzySet))) throw new NacException($"Fuzzy Set {fuzzyClause?.FuzzySet} Not Found in {tag.Name}");
            var fuzzySets = tag.FuzzySets.Select(fs => fs.GetFuzzySet());
            var lvStart = fuzzySets.Min(fs => fs.LeftLimit);
            var lvEnd = fuzzySets.Max(fs => fs.RightLimit);
            var lv = new LinguisticVariable(tag.Name, lvStart, lvEnd);
            foreach (var fs in fuzzySets) lv.AddLabel(fs);
            tagSet.Add(tag);
            lvDict.Add(lv.Name, lv);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nac.Fuzzy.Common {
    public class NacFuzzyRuleRegex : Regex {
        private static readonly string cSpace0 = @"\s*";
        private static readonly string cSpace1 = @"\s+";
        private static readonly string cIF = $@"\bIF\b";
        private static readonly string cTHEN = $@"\bTHEN\b";
        private static readonly string cnIFCond = "IfCond";
        private static readonly string cnThenOp = "ThenOp";
        private static readonly string cRule = $@"^{cSpace0}{cIF}{cSpace1}(?<{cnIFCond}>.+){cSpace1}{cTHEN}{cSpace1}(?<{cnThenOp}>.+){cSpace0}$";
        private static readonly string cAND = $@"(?:\bAND\b)";
        private static readonly string cOR = $@"(?:\bOR\b)";
        private static readonly string cNOT = $@"(?:\bNOT\b)";
        private static readonly string cOP = $@"{cAND}|{cOR}";
        private static readonly string cOPNOT = $@"{cAND}{cSpace1}{cNOT}|{cOR}{cSpace1}{cNOT}";
        private static readonly string cnLV = $@"LinVar";
        private static readonly string cLV = $@"(?<{cnLV}>\w+)";
        private static readonly string cnFS = $@"FuzzySet";
        private static readonly string cFS = $@"(?<{cnFS}>\w+)";
        private static readonly string cIS = $@"(?:\bIS\b)";
        private static readonly string cnCLA = $@"Clause";
        private static readonly string cCLA = $@"^(?<{cnCLA}>{cLV}{cSpace1}{cIS}{cSpace1}{cFS})$";
        private static readonly string cCLANOT = $@"^(?<{cnCLA}>{cLV}{cSpace1}{cIS}({cSpace1}{cNOT})?{cSpace1}{cFS})$";

        private string _text;
        public string Text {
            get { return _text; }
            private set {
                _text = value;
                _fuzzyRule = null;
                _inputs = null;
                _output = null;
            }
        }
        public NacFuzzyRuleRegex(string text) { Text = text; }

        public bool IsFormatValid { get { return FuzzyRule != null; } }
        public bool IsOutputValid { get { return _Output != null; } }
        public bool AreInputsValid { get { return _Inputs != null; } }


        public Tuple<string, string, string> Output {
            get {
                return new Tuple<string, string, string>(_Output.Groups[cnCLA].Value, _Output.Groups[cnLV].Value, _Output.Groups[cnFS].Value);
            }
        }
        public IEnumerable<Tuple<string, string, string>> Inputs {
            get {
                foreach(var input in _Inputs.OfType<Match>())
                    yield return new Tuple<string, string, string>(input.Groups[cnCLA].Value, input.Groups[cnLV].Value, input.Groups[cnFS].Value);
            }
        }

        private Match _fuzzyRule;
        private Match FuzzyRule {
            get {
                if (_fuzzyRule == null) {
                    var matches = Matches(Text, cRule, RegexOptions.IgnoreCase);
                    if (matches.Count == 1) _fuzzyRule = matches[0];
                }
                return _fuzzyRule;
            }
        }

        private string IfCond { get { return FuzzyRule?.Groups[cnIFCond].Value.Trim(); } }
        private string ThenOp { get { return FuzzyRule?.Groups[cnThenOp].Value.Trim(); } }

        private List<Match> _inputs;
        private List<Match> _Inputs {
            get {
                if (_inputs == null) {
                    var result = new List<Match>();
                    var splitNOT = Split(IfCond, cOPNOT, RegexOptions.IgnoreCase);
                    foreach (var split in splitNOT) {
                        foreach (var clause in Split(split.Trim(), cOP, RegexOptions.IgnoreCase)) {
                            var matches = Matches(clause.Trim(), cCLANOT, RegexOptions.IgnoreCase);
                            if (matches.Count != 1) return null;
                            result.Add(matches[0]);
                        }
                    }
                    _inputs = result;
                }
                return _inputs;
            }
        }
        private Match _output;
        private Match _Output {
            get {
                if (_output == null) {
                    var matches = Matches(ThenOp, cCLA, RegexOptions.IgnoreCase);
                    if (matches.Count == 1) _output = matches[0];
                }
                return _output;
            }
        }
    }
}

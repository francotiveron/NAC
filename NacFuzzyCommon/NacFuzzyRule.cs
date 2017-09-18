using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static NacGlobal;

namespace Nac.Fuzzy.Common {
    public class NacFuzzyRule {
        private string _content;

        public string Content {
            get { return _content; }
            set {
                if (value != _content) {
                    _content = value;
                    SyntaxParsingResult = SyntaxParsingResults.Unparsed;
                    Parse();
                }
            }
        }


        public bool IsSyntaxValid { get { return SyntaxParsingResult == SyntaxParsingResults.Parsed; } }

        //public bool IsSemanticValid(NacDatabase database) {
        //    foreach (var fuzzyClause in FuzzyClauses) {
        //        if (fuzzyClause == null) return false;
        //        var tag = database.Find(fuzzyClause?.LinVar) as NacTag;
        //        if (tag == null || tag.FuzzySets.All(fuzzySet => !fuzzySet.Name.Equals(fuzzyClause?.FuzzySet))) return false;
        //    }
        //    return true;
        //}

        public struct FuzzyClause {
            public string Clause;
            public string LinVar;
            public string FuzzySet;
        }

        public IEnumerable<FuzzyClause?> FuzzyInputs { get; private set; }
        public FuzzyClause? FuzzyOutput { get; private set; }

        public IEnumerable<FuzzyClause?> FuzzyClauses {
            get {
                yield return FuzzyOutput;
                foreach (var term in FuzzyInputs) yield return term;
            }
        }

        public NacFuzzyRule(string text = null) { Content = text; }

        public enum SyntaxParsingResults { Unparsed, Parsing, ParsingException, RuleError, InputsError, OutputError, Parsed }
        public SyntaxParsingResults SyntaxParsingResult { get; private set; }

        private void Parse() {
            try {
                SyntaxParsingResult = SyntaxParsingResults.Parsing;
                NacFuzzyRuleRegex regex = new NacFuzzyRuleRegex(Content);
                if (!regex.IsFormatValid) { SyntaxParsingResult = SyntaxParsingResults.RuleError; return; }
                if (!regex.AreInputsValid) { SyntaxParsingResult = SyntaxParsingResults.InputsError; return; }
                if (!regex.IsOutputValid) { SyntaxParsingResult = SyntaxParsingResults.OutputError; return; }
                SyntaxParsingResult = SyntaxParsingResults.Parsed;
                FuzzyOutput = new FuzzyClause() { Clause = regex.Output.Item1, LinVar = regex.Output.Item2, FuzzySet = regex.Output.Item3 };
                FuzzyInputs = regex.Inputs.Select(input => new FuzzyClause() { Clause = input.Item1, LinVar = input.Item2, FuzzySet = input.Item3 }).Cast<FuzzyClause?>();
            }
            catch (Exception x) when (G.Log(x, this)) {
                SyntaxParsingResult = SyntaxParsingResults.ParsingException;
                FuzzyInputs = null; FuzzyOutput = null;
            }
        }

        public override string ToString() {
            return Content;
        }
    }
}

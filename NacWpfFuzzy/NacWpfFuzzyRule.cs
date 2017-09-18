using Nac.Fuzzy.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Nac.Wpf.Fuzzy {
    partial class NacWpfFuzzyRule : INotifyPropertyChanged {
        public NacFuzzyRule Base { get; set; }

        public NacWpfFuzzyRule(string rule) { Base = new NacFuzzyRule(rule); }
        public NacWpfFuzzyRule() : this("IF input IS fuzzySet1 THEN output IS fuzzySet2") { }

        public string Content {
            get { return Base.Content; }
            set {
                if (value != Base.Content) {
                    Base.Content = value;
                    OnPropertyChanged("Content");
                    OnPropertyChanged("Self");
                    OnPropertyChanged("IsSyntaxValid");
                }
            }
        }

        public bool IsSyntaxValid { get { return Base.IsSyntaxValid; } }
        public NacWpfFuzzyRule Self { get { return this; } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static implicit operator string(NacWpfFuzzyRule rule) { return rule.Content; }
        public static implicit operator NacWpfFuzzyRule(string rule) { return new NacWpfFuzzyRule(rule); }

        public override string ToString() {
            return Base.ToString();
        }

    }
    class NacWpfFuzzyRuleValidator : ValidationRule {
        public NacWpfFuzzyRuleValidator() : base(ValidationStep.CommittedValue, true) { }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
            var bindingExpression = value as BindingExpression;
            var wpfRule = bindingExpression.DataItem as NacWpfFuzzyRule;
            return new ValidationResult(wpfRule == null ? false : wpfRule.Base.IsSyntaxValid, null);
        }
    }
    class NacWpfFuzzyRuleDiagnosticConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var wpfRule = (NacWpfFuzzyRule)value;
            return wpfRule?.DiagnosticText;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    partial class NacWpfFuzzyRule {
        public string DiagnosticText {
            get {
                return string.Join(Environment.NewLine.ToString(),
                new string[] {
                        Content
                        , $"{nameof(Base.SyntaxParsingResult)}: {Base.SyntaxParsingResult.ToString()}"
                        , $"Inputs: {string.Join(", ", Base.FuzzyInputs.Select(fi => $"[{fi?.LinVar}, {fi?.FuzzySet}] => {fi?.Clause}"))}"
                        , $"Output: {$"[{Base.FuzzyOutput?.LinVar}, {Base.FuzzyOutput?.FuzzySet}] => {Base.FuzzyOutput?.Clause}"}"
                }
                );
            }
        }
    }

}

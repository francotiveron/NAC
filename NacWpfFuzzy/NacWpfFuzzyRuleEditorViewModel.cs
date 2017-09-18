using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wpf.Fuzzy {
    class NacWpfFuzzyRuleEditorViewModel : ObservableCollection<NacWpfFuzzyRule> {
        private NacWpfFuzzyRule _current = new NacWpfFuzzyRule();
        public NacWpfFuzzyRule Current { get { return _current; } set { if (value != null) { _current = value; OnPropertyChanged(new PropertyChangedEventArgs("Current")); } } }

        public NacWpfFuzzyRuleEditorViewModel() {  }
        public NacWpfFuzzyRuleEditorViewModel(HashSet<string> rules) : base(rules.Select(rule => new NacWpfFuzzyRule(rule))) { }
    }
}

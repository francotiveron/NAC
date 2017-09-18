using Nac.Common.Fuzzy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wpf.Fuzzy {
    class NacWpfFuzzySetEditorViewModel : ObservableCollection<NacWpfFuzzySet> {
        public NacWpfFuzzySetEditorViewModel(HashSet<NacFuzzySet> sets) : base(sets.Select(set => new NacWpfFuzzySet(set))) { }
        public NacWpfFuzzySetEditorViewModel() { }
    }
}

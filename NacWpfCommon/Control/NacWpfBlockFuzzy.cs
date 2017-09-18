using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wpf.Common.Control {
    public class NacWpfBlockFuzzy : NacWpfBlockSeq {
        public NacWpfBlockFuzzy(NacBlockFuzzy nacBlockFuzzy) : base(nacBlockFuzzy) { }
        public new NacBlockFuzzy Base { get { return base.Base as NacBlockFuzzy; } }

        public HashSet<string> FuzzyRules {
            get { return Base.FuzzyRules; }
            set { if (value != Base.FuzzyRules) { Base.FuzzyRules = value; OnNotifyPropertyChanged("FuzzyRules"); } }
        }

    }
}

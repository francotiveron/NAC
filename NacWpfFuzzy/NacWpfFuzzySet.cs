using Accord.Fuzzy;
using Nac.Common.Fuzzy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wpf.Fuzzy {
    class NacWpfFuzzySet : INotifyPropertyChanged {
        public NacFuzzySet Base { get; set; }

        public NacWpfFuzzySet(NacWpfTrapezoidalMembershipFunction mf, string name) {
            Base = new NacFuzzySet(mf.Base, name);
            MembershipFunction = mf;
        }

        public NacWpfFuzzySet(NacWpfTrapezoidalMembershipFunction mf) {
            Base = new NacFuzzySet(mf.Base);
            MembershipFunction = mf;
        }

        public NacWpfFuzzySet() : this(new NacWpfTrapezoidalMembershipFunction()) {}

        public NacWpfFuzzySet(NacFuzzySet set) : this(new NacWpfTrapezoidalMembershipFunction(set.MembershipFunction as NacTrapezoidalMembershipFunction), set.Name) { }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get { return Base.Name; } set { if (value != Base.Name) { Base.Name = value; OnNotifyPropertyChanged("Name"); } } }

        private NacWpfTrapezoidalMembershipFunction _membershipFunction;
        public NacWpfTrapezoidalMembershipFunction MembershipFunction {
            get { return _membershipFunction; }
            set {
                if (value != _membershipFunction) {
                    _membershipFunction = value;
                    MembershipFunction.PropertyChanged += MembershipFunction_PropertyChanged;
                }
            }
        }

        private void MembershipFunction_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            OnNotifyPropertyChanged("WpfMembershipFunction");
        }

        public void OnNotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() {
            return $"{Name} ({MembershipFunction})";
        }
    }
}

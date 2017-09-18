using Accord.Fuzzy;
using Nac.Common.Fuzzy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wpf.Fuzzy {
    class NacWpfTrapezoidalMembershipFunction: INotifyPropertyChanged {
        public NacTrapezoidalMembershipFunction Base { get; set; }
        public NacWpfTrapezoidalMembershipFunction() { Base = new NacTrapezoidalMembershipFunction(); }
        public NacWpfTrapezoidalMembershipFunction(NacTrapezoidalMembershipFunction mf) { Base = mf; }

        public event PropertyChangedEventHandler PropertyChanged;

        public float m1 { get { return Base.m1; } set { if (value != Base.m1) { Base.m1 = value; OnNotifyPropertyChanged("m1"); } } }
        public float m2 { get { return Base.m2; } set { if (value != Base.m2) { Base.m2 = value; OnNotifyPropertyChanged("m2"); } } }
        public float m3 { get { return Base.m3; } set { if (value != Base.m3) { Base.m3 = value; OnNotifyPropertyChanged("m3"); } } }
        public float m4 { get { return Base.m4; } set { if (value != Base.m4) { Base.m4 = value; OnNotifyPropertyChanged("m4"); } } }
        public float min { get { return Base.min; } set { if (value != Base.min) { Base.min = value; OnNotifyPropertyChanged("min"); } } }
        public float max { get { return Base.max; } set { if (value != Base.max) { Base.max = value; OnNotifyPropertyChanged("max"); } } }
        public int edge { get { return Base.edge; } set { if (value != Base.edge) { Base.edge = value; OnNotifyPropertyChanged("edge"); } } }

        public void OnNotifyPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString() {
            return $"{m1},{m2},{m3},{m4},[{min}..{max}]{((new string[] { ",Left", "", ",Right" })[edge + 1])}";
        }

    }
}

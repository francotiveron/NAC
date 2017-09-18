using Nac.Common;
using Nac.Common.Control;
using Nac.Common.Fuzzy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Nac.Wpf.Common.Control {
    [Serializable]
    public class NacWpfTag : NacWpfNamedObject {
        public NacWpfTag(NacTag nacTag) : base(nacTag) { }
        public new NacTag Base { get { return base.Base as NacTag; } }

        public string Description {
            get { return Base.Description; }
            set { if (value != Base.Description) { Base.Description = value; OnNotifyPropertyChanged("Description"); } }
        }

        public NacTagScope Scope {
            get { return Base.Scope; }
            set { if (value != Base.Scope) { Base.Scope = value; OnNotifyPropertyChanged("Scope"); } }
        }

        public string Address {
            get { return Base.Address; }
            set { if (value != Base.Address) { Base.Address = value; OnNotifyPropertyChanged("Address"); } }
        }

        public string Formula {
            get { return Base.Formula; }
            set { if (value != Base.Formula) { Base.Formula = value; OnNotifyPropertyChanged("Formula"); } }
        }

        [ExpandableObject]
        public NacTagValue Tag {
            get { return Base.Tag; }
            set {
                if (value != Base.Tag) {
                    NacTagValue old = Base.Tag;
                    Base.Tag = value; OnNotifyPropertyChanged("Tag");
                    if (old.Value != Base.Value) OnNotifyPropertyChanged("Value");
                    if (old.Quality != Base.Quality) OnNotifyPropertyChanged("Quality");
                    if (old.Timestamp != Base.Timestamp) OnNotifyPropertyChanged("Timestamp");
                }
            }
        }

        public double Value {
            get { return Tag.Value; }
            set { if (value != Tag.Value) { Tag = new NacTagValue(Tag.Quality, Tag.Timestamp, value); } }
        }
        public NacValueQuality Quality {
            get { return Tag.Quality; }
            set { if (value != Tag.Quality) Tag = new NacTagValue(value, Tag.Timestamp, Tag.Value); }
        }

        public DateTime Timestamp {
            get { return Tag.Timestamp; }
            set { if (value != Tag.Timestamp) Tag = new NacTagValue(Tag.Quality, value, Tag.Value); }
        }

        public HashSet<NacFuzzySet> FuzzySets {
            get { return Base.FuzzySets; }
            set { if (value != Base.FuzzySets) { Base.FuzzySets = value; OnNotifyPropertyChanged("FuzzySets"); } }
        }

        public TimeSpan History {
            get { return Base.History; }
            set { if (value != Base.History) { Base.History = value; OnNotifyPropertyChanged("History"); } }
        }
    }
}

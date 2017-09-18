using Nac.Common.Control;
using System.Windows;
using Nac.Common;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;
using System;

namespace Nac.Wpf.Common.Control {
    public class NacWpfBlock : NacWpfObject {
        public NacWpfBlock(NacBlock nacBlock) : base(nacBlock) { }
        public new NacBlock Base { get { return base.Base as NacBlock; } }

        public string Description {
            get { return Base.Description; }
            set { if (value != Base.Description) { Base.Description = value; OnNotifyPropertyChanged("Description"); } }
        }
        public string Code {
            get { return Base.Code; }
            set { if (value != Base.Code) { Base.Code = value; OnNotifyPropertyChanged("Code"); } }
        }

        public Point Position {
            get { return Base.Position; }
            set { if (value != Base.Position) { Base.Position = value; OnNotifyPropertyChanged("Position"); } }
        }

        [ExpandableObject]
        public NacExecutionStatus Status {
            get { return Base.Status; }
            set {
                if (value != Base.Status) {
                    NacExecutionStatus old = Base.Status;
                    Base.Status = value; OnNotifyPropertyChanged("Status");
                    if (old.Quality != Base.Quality) OnNotifyPropertyChanged("Quality");
                    if (old.Scheduled != Base.Scheduled) OnNotifyPropertyChanged("Scheduled");
                    if (old.Countdown != Base.Countdown) OnNotifyPropertyChanged("Countdown");
                }
            }
        }

        public NacExecutionQuality Quality {
            get { return Status.Quality; }
            set { if (value != Status.Quality) Status = new NacExecutionStatus(Status.Scheduled, value, Status.Countdown); }
        }

        public bool Scheduled {
            get { return Status.Scheduled; }
            set { if (value != Status.Scheduled) Status = new NacExecutionStatus(value, Status.Quality, Status.Countdown); }
        }

        public TimeSpan Countdown {
            get { return Status.Countdown; }
            set { if (value != Status.Countdown) Status = new NacExecutionStatus(Status.Scheduled, Status.Quality, value); }
        }
    }
}

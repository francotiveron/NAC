using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common;
using Nac.Common.Control;
using Nac.Wpf.Common.Control;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.IO;

namespace Nac.Wpf.Common.Control {
    public class NacWpfProject : NacWpfObjectWithChildren {
        public NacWpfProject(NacProject nacProject) : base(nacProject) { }
        public new NacProject Base { get { return base.Base as NacProject; } }

        private NacWpfDatabase _database;
        public NacWpfDatabase Database {
            get {
                if (_database == null) _database = new NacWpfDatabase((Base as NacProject).Database);
                return _database;
            }
        }

        public new string Name {
            get { return base.Name; }
            set {
                base.Name = value;
                Database.OnNotifyPropertyChanged("Name");
            }
        }

        public bool WriteOutputs {
            get { return Base.WriteOutputs; }
            set { if (value != Base.WriteOutputs) { Base.WriteOutputs = value; OnNotifyPropertyChanged("WriteOutputs"); } }
        }

        public bool ReadInputs {
            get { return Base.ReadInputs; }
            set { if (value != Base.ReadInputs) { Base.ReadInputs = value; OnNotifyPropertyChanged("ReadInputs"); } }
        }

        public bool Run {
            get { return Base.Run; }
            set { if (value != Base.Run) { Base.Run = value; OnNotifyPropertyChanged("Run"); } }
        }

        public void Save() {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".nacp";
            dlg.Filter = "Nac projects (.nacp)|*.nacp";

            var result = dlg.ShowDialog();

            if (result == true) {
                string filename = dlg.FileName;
                IEnumerable<NacObject> toSave = Flat();
                string xml = NacSerializer.Serialize(toSave);
                File.WriteAllText(filename, xml);
            }
        }
    }
}

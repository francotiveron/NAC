using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Field.Citect.WindowsService {
    public partial class HostService : ServiceBase {
        public HostService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            NacFieldCitectContext.Create();
            NacFieldCitectWcfServer.Start($@"net.tcp://{NacFieldCitectContext.IP}:65457");
        }

        protected override void OnStop() {
            NacFieldCitectWcfServer.Stop();
            NacFieldCitectContext.Destroy();
        }
    }
}

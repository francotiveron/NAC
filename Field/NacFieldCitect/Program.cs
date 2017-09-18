using Nac.Field.Citect.WindowsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nac.Field.Citect {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
            if (Environment.UserInteractive) {
                NacFieldCitectContext.Create();
                NacFieldCitectWcfServer.Start($@"net.tcp://{NacFieldCitectContext.IP}:65457");
                var f = new Form1();
                Application.Run(f);
                NacFieldCitectWcfServer.Stop();
                NacFieldCitectContext.Destroy();
            }
            else {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new HostService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}

using Nac.Engine.WindowsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NacEngineGlobal;

namespace Nac.Engine {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            if (Environment.UserInteractive) {
                NacEngineContext.Create();
                NacEngineWcfServer.Start($@"net.tcp://{NacEngineContext.IP}:65456");
                var f = new Form1();
                Application.Run(f);
                NacEngineWcfServer.Stop();
                NacEngineContext.Destroy();
            }
            else {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new HostService()
                };
                System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
                ServiceBase.Run(ServicesToRun);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            G?.Log(e);
        }
    }
}

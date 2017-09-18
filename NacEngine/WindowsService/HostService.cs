using System.ServiceProcess;
using System.Threading;
using Startup = Nac.Engine.NacEngStartupParser;

namespace Nac.Engine.WindowsService {
    partial class HostService : ServiceBase {
        public HostService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            //CLAP.Parser.Run<Startup>(args);
            //if (args.Length > 0) {
            //bool b = true;
            //while (b) Thread.Sleep(100);
            //}
            NacEngineContext.Create();
            NacEngineWcfServer.Start($@"net.tcp://{NacEngineContext.IP}:65456");
        }

        protected override void OnStop() {
            NacEngineWcfServer.Stop();
            NacEngineContext.Destroy();
        }
    }
}

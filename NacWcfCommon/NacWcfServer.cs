using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wcf.Common {
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class NacWcfServer<TInterface, TImplementation> where TImplementation : class, TInterface {
        private static Uri _uri;
        private static ServiceHost _wcfHost;

        public static void Start(string url) {
            _uri = new Uri(url);
            Init();
        }

        private static void Init() {
            NetTcpBinding netTcpBinding = new NetTcpBinding();
            ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();

            _wcfHost = new ServiceHost(typeof(TImplementation), _uri);
            _wcfHost.Description.Behaviors.Add(serviceMetadataBehavior);
            _wcfHost.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
            _wcfHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
            _wcfHost.AddServiceEndpoint(typeof(TInterface), netTcpBinding, _uri);
            _wcfHost.Faulted += _wcfHost_Faulted;
            _wcfHost.Open();
        }

        private static void _wcfHost_Faulted(object sender, EventArgs e) {
            _wcfHost?.Abort();
            Init();
        }

        public static void Stop() {
            _wcfHost?.Close();
            _wcfHost = null;
        }
    }
}

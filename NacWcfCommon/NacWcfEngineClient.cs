using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common;
using Nac.Common.Control;
using System.ServiceModel;
using static NacGlobal;
using System.Windows;

public delegate void NewEngineMessageHandler(string[] messages);
namespace Nac.Wcf.Common {
    public class NacWcfEngineClient : NacWcfClient<INacWcfEngine>, INacWcfEngine {

        public NacWcfEngineClient(string url) : base(url) { }

        public NacWcfEngineCallback CallbackObject { get; private set; } = new NacWcfEngineCallback();

        #region INacWcfEngine
        protected override INacWcfEngine CreateChannel() {
            var ctx = new InstanceContext(CallbackObject);
            return (new DuplexChannelFactory<INacWcfEngine>(ctx, Binding, EndPoint)).CreateChannel();
        }

        public void Add(IEnumerable<NacObject> list) {
            Call((delegate (INacWcfEngine client) {
                client.Add(list);
            }));
        }

        public void Connect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector) {
            Call((delegate (INacWcfEngine client) {
                client.Connect(blockPathSource, blockPathDestination, sourceConnector, destinationConnector);
            }));
        }

        public void Delete(string[] paths) {
            Call((delegate (INacWcfEngine client) {
                client.Delete(paths);
            }));
        }

        public void Disconnect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector) {
            Call((delegate (INacWcfEngine client) {
                client.Disconnect(blockPathSource, blockPathDestination, sourceConnector, destinationConnector);
            }));
        }

        public NacCatalog<NacObject> GetCatalog() {
            return Call((delegate (INacWcfEngine client) {
                return client.GetCatalog();
            })) as NacCatalog<NacObject>;
        }

        public Dictionary<string, NacTagValue> GetRDTBValues(string rtdbPath) {
            return Call((delegate (INacWcfEngine client) {
                return client.GetRDTBValues(rtdbPath);
            })) as Dictionary<string, NacTagValue>;
        }

        public Dictionary<string, NacExecutionStatus> GetSectionStates(string sectionPath) {
            return Call((delegate (INacWcfEngine client) {
                return client.GetSectionStates(sectionPath);
            })) as Dictionary<string, NacExecutionStatus>;
        }

        public Dictionary<string, NacTagValue> GetValues(string[] tagPaths) {
            return Call((delegate (INacWcfEngine client) {
                return client.GetValues(tagPaths);
            })) as Dictionary<string, NacTagValue>;
        }

        public void Update(string[] paths, string property, NacWcfObject[] values) {
            Call((delegate (INacWcfEngine client) {
                client.Update(paths, property, values);
            }));
        }
        #endregion
    }

    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class NacWcfEngineCallback : INacWcfEngineCallback {
        public event NewEngineMessageHandler NewEngineMessageEvent;

        Task _broadcastTask;
        public void Broadcast(string[] messages) {
            if (NewEngineMessageEvent != null)
                NacUtils.Decouple(() => NewEngineMessageEvent.Invoke(messages), ref _broadcastTask);
        }

        public void Dispose() {
            
        }
    }

}

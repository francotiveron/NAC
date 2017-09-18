using Nac.Common;
using Nac.Common.Control;
using Nac.Wcf.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading.Tasks;
using static NacEngineGlobal;

namespace Nac.Engine {
    public class NacEngineWcfServer : NacWcfServer<INacWcfEngine, NacEngineWcfServer>, INacWcfEngine, INacWcfEngineCallback {
        private const int cMaxCachedMessages = 10;

        INacWcfEngineCallback _callback;
        public NacEngineWcfServer() {
            _callback = OperationContext.Current?.GetCallbackChannel<INacWcfEngineCallback>();
            G.NacEngineEvent += G_NacEngineEvent;
        }

        private List<string> CachedMessages = new List<string>();
        private DateTime _lastBroadcast = DateTime.Now;
        private void G_NacEngineEvent(object sender, NacEngineEventArgs args) {
            if (args.OriginalArgs is NacEngineLogger.NacEngineLogEventArgs) {
                var orgArgs = args.OriginalArgs as NacEngineLogger.NacEngineLogEventArgs;
                if (CachedMessages.Count >= cMaxCachedMessages) CachedMessages.RemoveAt(cMaxCachedMessages - 1);
                CachedMessages.Add(orgArgs.message);
                if (CachedMessages.Count > 0 && (DateTime.Now - _lastBroadcast).TotalSeconds >= 1) {
                    Broadcast(CachedMessages.ToArray());
                    CachedMessages.Clear();
                }
            }
        }

        public void Dispose() {
            G.NacEngineEvent -= G_NacEngineEvent;
        }

        private NacEngine Engine { get { return G.Engine; } }

        public void Update(string[] paths, string property, NacWcfObject[] values) {
            Engine.Update(paths, property, values);
        }


        public void Add(IEnumerable<NacObject> list) {
            Engine.Add(list);
        }


        public NacCatalog<NacObject> GetCatalog() {
            return Engine.GetCatalog();
        }

        public Dictionary<string, NacTagValue> GetValues(string[] tagPaths) {
            return Engine.GetValues(tagPaths);
        }

        public Dictionary<string, NacTagValue> GetRDTBValues(string rtdbPath) {
            return Engine.GetRTDBValues(rtdbPath);
        }

        public Dictionary<string, NacExecutionStatus> GetSectionStates(string sectionPath) {
            return Engine.GetSectionStates(sectionPath);
        }

        public void Delete(string[] paths) {
            Engine.Delete(paths);
        }

        public void Connect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector) {
            Engine.Connect(blockPathSource, blockPathDestination, sourceConnector, destinationConnector);
        }

        public void Disconnect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector) {
            Engine.Disconnect(blockPathSource, blockPathDestination, sourceConnector, destinationConnector);
        }

        private Task _broadcastTask;
        public void Broadcast(string[] messages) {
            NacUtils.Decouple(() =>_callback.Broadcast(messages), ref _broadcastTask);
        }
    }
}

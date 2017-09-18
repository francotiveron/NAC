using Nac.Common.Control;
using System.ServiceModel;
using static NacUtils;

namespace Nac.Wcf.Common {
    public class NacWcfFieldClient : NacWcfClient<INacWcfField>, INacWcfField {
        public NacWcfFieldClient(string url) : base(url) {  }

        public NacWcfFieldCallback CallbackObject { get; private set; } = new NacWcfFieldCallback();

        protected override INacWcfField CreateChannel() {
            var ctx = new InstanceContext(CallbackObject);
            return (new DuplexChannelFactory<INacWcfField>(ctx, Binding, EndPoint)).CreateChannel();
        }


        #region INacWcfField
        public byte[] Browse(string filter = null) {
            return Call((delegate (INacWcfField client) {
                return client.Browse(filter);
            })) as byte[];
        }

        //public NacTagValue? GetTagVal(string tagName) {
        //    return (NacTagValue?)Call((delegate (INacWcfField client) {
        //        return client.GetTagVal(tagName);
        //    }));
        //}

        public NacTagValue[] Read(string[] tagNames) {
            return (NacTagValue[])Call((delegate (INacWcfField client) {
                return client.Read(tagNames);
            }));
        }

        public bool[] Write(string[] tagNames, NacTagValue[] tagValues) {
            return (bool[])Call((delegate (INacWcfField client) {
                return client.Write(tagNames, tagValues);
            }));
        }
        #endregion
    }
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class NacWcfFieldCallback : INacWcfFieldCallback {
        public void Dispose() { }

        public void Dummy() { }
    }
}

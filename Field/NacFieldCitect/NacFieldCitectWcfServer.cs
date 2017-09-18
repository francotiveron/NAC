using Nac.Wcf.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common.Control;
using Qualities = Nac.Common.Control.NacValueQuality;
using static NacFieldCitectGlobal;
using static NacUtils;
using System.ServiceModel;

namespace Nac.Field.Citect {
    class NacFieldCitectWcfServer : NacWcfServer<INacWcfField, NacFieldCitectWcfServer>, INacWcfField, INacWcfFieldCallback {
        private NacFieldCitectProxy Citect { get { return G.Citect; } }

        INacWcfFieldCallback _callback;
        public NacFieldCitectWcfServer() {
            _callback = OperationContext.Current.GetCallbackChannel<INacWcfFieldCallback>();
        }

        public void Dispose() { }

        public void Dummy() { }

        private NacTagValue Convert(bool tagOK, string sValue) {
            DateTime timestamp = DateTime.Now;
            Qualities quality = Qualities.Unknown;
            double value = 0d;

            if (tagOK) {
                double d;
                if (double.TryParse(sValue, out d)) {
                    quality = Qualities.Good;
                    value = d;
                } else {
                    quality = Qualities.Uncertain;
                    value = double.NaN;
                }
            } else quality = Qualities.Bad;

            return new NacTagValue(quality, timestamp, value);
        }

        //public NacTagValue GetTagVal(string tagName) {
        //    string sValue;
        //    return Convert(Citect.Read(tagName, out sValue), sValue);
        //}

        public NacTagValue[] Read(string[] tagNames) {
            string[] sValues; NacTagValue[] ret;

            bool readOK = Citect.Read(tagNames, out sValues);
            if (readOK) ret = sValues.Foreach(sValue => Convert(sValue != "?", sValue)).ToArray();
            else {
                NacTagValue badTag = new NacTagValue(Qualities.Bad, DateTime.Now, double.NaN);
                ret = Enumerable.Repeat(badTag, tagNames.Length).ToArray();
            }

            return ret;
        }

        public bool[] Write(string[] tagNames, NacTagValue[] tagValues) {
            bool[] ret;

            bool writeOk = Citect.Write(tagNames, tagValues.Select(tagValue => tagValue.Value.ToString()).ToArray(), out ret);
            if (writeOk) return ret;
            return null;
        }

        public byte[] Browse(string filter = null) {
            return Compress(Citect.Browse(filter), @"#");
        }
    }
}

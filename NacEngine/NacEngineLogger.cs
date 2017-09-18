using Nac.Common;
using Nac.Wcf.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Engine {
    internal class NacEngineLogger : NacLogger {
        private const string cDefaultLogFile = "NacEngine.log";
        private IDevice[] _devices;

        public NacEngineLogger() {
            _devices = new IDevice[] { new FileDevice(), new BroadcastDevice(this) };
        }
        private class FileDevice : IStorageDevice {
            public void Print(string[] messages) {
                lock (this) File.AppendAllLines(cDefaultLogFile, messages);
            }
        }

        public class NacEngineLogEventArgs : EventArgs {
            public string message;
        }
        public delegate void NacEngineLogEventHandler(object sender, NacEngineLogEventArgs args);
        public event NacEngineLogEventHandler NewLogMessage;

        private class BroadcastDevice : IInteractiveDevice {
            private NacEngineLogger _logger;
            public BroadcastDevice(NacEngineLogger logger) { _logger = logger; }
            public void Print(string message) {
                _logger.NewLogMessage?.Invoke(this, new NacEngineLogEventArgs() { message = message });
            }
        }

        protected override IEnumerable<IDevice> Devices { get { return _devices; } }
    }
}

using Nac.Common;
using Nac.Wcf.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nac.Common.NacLogger;

namespace Nac.Field.Citect {
    internal class NacFieldCitectLogger : NacLogger {
        private const string cDefaultLogFile = "NacFieldCitect.log";
        private IDevice[] _devices;

        public NacFieldCitectLogger() {
            _devices = new IDevice[] { new FileDevice() };
        }
        private class FileDevice : IStorageDevice {
            public void Print(string[] messages) {
                lock (this) File.AppendAllLines(cDefaultLogFile, messages);
            }
        }
        protected override IEnumerable<IDevice> Devices { get { return _devices; } }
    }
}

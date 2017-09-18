using CLAP;
using Nac.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Engine {
    class NacEngStartupParser {
        public static bool NoService { get; set; }
        public static bool NoWcf { get; set; }
        [Verb(IsDefault = true)]
        protected static void StartupParameters(bool noService, bool noWcf) {
            NoService = noService;
            NoWcf = noWcf;
        }
    }
}

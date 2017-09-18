using Nac.Field.Citect;

public interface INacFieldCitectContext : INacContext {
    NacFieldCitectProxy Citect { get; }
}

public static class NacFieldCitectGlobal {
    public static INacFieldCitectContext G { get; set; }
}

namespace Nac.Field.Citect {
    using System;
    using static NacFieldCitectGlobal;

    public class NacFieldCitectContext : INacFieldCitectContext {
        private const string cCitectUser = "Administrator";
        private const string cCitectPassword = "alfa1";
        public static string IP { get; private set; }

        #region static
        private static NacFieldCitectContext Instance { get { return G as NacFieldCitectContext; } }
        public static void Create() {
            Logger = new NacFieldCitectLogger();
            IP = NacUtils.FindIP(); if (IP == null) IP = NacUtils.cLoopbackIP;

            G = new NacFieldCitectContext();
            Instance.Citect = new NacFieldCitectProxy();
        }
        public static void Destroy() {
            Instance.Dispose();
            Logger.Dispose();
        }
        #endregion
        private static NacFieldCitectLogger Logger { get; set; }

        public void Dispose() {}

        public bool Log(params object[] args) {
            return Logger.Log(args);
        }

        public NacFieldCitectProxy Citect { get; private set; }
    }
}

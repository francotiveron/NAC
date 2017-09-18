using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common;
using System.IO;
using System.Runtime.Serialization;
using Nac.Engine;
using Nac.Engine.Control;

public class NacEngineEventArgs : EventArgs {
    public object OriginalSender;
    public EventArgs OriginalArgs;
}
public delegate void NacEngineEventHandler(object sender, NacEngineEventArgs args);
public interface INacEngineContext : INacContext {
    NacEngine Engine { get; }
    NacEngineField Field { get; }
    NacEngineRuntime Runtime(string path);
    NacEngineProject Project(string path);
    NacEngineDatabase Database(string path);
    event NacEngineEventHandler NacEngineEvent;
}
public static class NacEngineGlobal {
    public static INacEngineContext G { get { return NacGlobal.G as NacEngineContext; } set { NacGlobal.G = value; } }
}

namespace Nac.Engine {
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using static NacEngineGlobal;

    [DataContract]
    public class NacEngineContext : INacEngineContext {
        private const string contextFileName = "NacEngineContext.Xml";
        public static string IP { get; private set; }

        private NacEngineContext(NacEngine engine) { Engine = engine; }

        #region INacContext
        [DataMember]
        public NacEngine Engine { get; private set; }

        public NacEngineField Field { get; private set; }

        public NacEngineProject Project(string path) {
            return Engine[NacPath.Parse(path).ProjectPath] as NacEngineProject;
        }
        public NacEngineDatabase Database(string path) {
            return Project(path).Database;
        }
        public NacEngineRuntime Runtime(string path) {
            return Project(path).Runtime;
        }

        private static NacEngineLogger Logger { get; set; }
        public bool Log(params object[] args) {
            return Logger.Log(args);
        }

        #endregion

        #region static
        private static NacEngineContext Instance { get { return G as NacEngineContext; } set { G = value; } }

        public static void Create() {
            Logger = new NacEngineLogger();
            IP = NacUtils.FindIP(); if (IP == null) IP = NacUtils.cLoopbackIP;
            Logger.Log(IP);
            try {
                Instance = Load() as NacEngineContext;
                Instance.Engine.Cleanup();
            } catch { Instance = new NacEngineContext(new NacEngine()); }

            Logger.NewLogMessage += Instance.Logger_NewLogMessage;
            Instance.Field = new NacEngineField(IP);
            Instance.Start();
        }


        public event NacEngineEventHandler NacEngineEvent;

        private void Logger_NewLogMessage(object sender, NacEngineLogger.NacEngineLogEventArgs args) {
            if (NacEngineEvent != null) {
                var newArgs = new NacEngineEventArgs() { OriginalSender = sender, OriginalArgs = args };
                NacEngineEvent.Invoke(Instance, newArgs);
            }
        }

        private void Start() { Engine.Start(); }

        public static void Destroy() {
            Instance.Dispose();
            Logger.Dispose();
        }

        private static void Save() {
            Instance.Engine.Cleanup();
            string xml = NacSerializer.Serialize(Instance);
            File.WriteAllText(contextFileName, xml);
        }

        private static object Load() {
            return NacSerializer.Deserialize(File.ReadAllText(contextFileName), typeof(NacEngineContext)) as NacEngineContext;
        }

        #endregion

        public void Dispose() {
            Engine.Dispose();
            Save();
        }
    }
}

using Nac.Common;
using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Nac.Wcf.Common {
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(INacWcfEngineCallback))]
    public interface INacWcfEngine {
        [OperationContract]
        void Delete(string[] paths);
        [OperationContract]
        void Add(IEnumerable<NacObject> list);
        [OperationContract]
        NacCatalog<NacObject> GetCatalog();
        [OperationContract]
        void Update(string[] paths, string property, NacWcfObject[] values);
        [OperationContract]
        Dictionary<string, NacTagValue> GetValues(string[] tagPaths);
        [OperationContract]
        Dictionary<string, NacTagValue> GetRDTBValues(string rtdbPath);
        [OperationContract]
        Dictionary<string, NacExecutionStatus> GetSectionStates(string sectionPath);
        [OperationContract]
        void Connect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector);
        [OperationContract]
        void Disconnect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector);
    }

    public interface INacWcfEngineCallback : IDisposable {
        [OperationContract(IsOneWay = true)]
        void Broadcast(string[] messages);
    }
}


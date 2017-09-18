using Nac.Common.Control;
using System;
using System.ServiceModel;

namespace Nac.Wcf.Common {
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(INacWcfFieldCallback))]
    public interface INacWcfField {
        //[OperationContract]
        //NacTagValue GetTagVal(string tagName);
        [OperationContract]
        NacTagValue[] Read(string[] tagNames);
        [OperationContract]
        bool[] Write(string[] tagNames, NacTagValue[] tagValues);
        [OperationContract]
        byte[] Browse(string filter = null);
    }

    public interface INacWcfFieldCallback : IDisposable {
        [OperationContract(IsOneWay = true)]
        void Dummy();
    }
}

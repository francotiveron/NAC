using Nac.Common;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using static NacGlobal;

namespace Nac.Wcf.Common {
    public class NacWcfClient<IContract> : IDisposable where IContract : class {
        protected NetTcpBinding Binding { get; private set; }
        protected EndpointAddress EndPoint { get; private set; }

        public NacWcfClient(string url) {
            Binding = new NetTcpBinding();
            //Binding.OpenTimeout = TimeSpan.FromHours(1.0);
            //Binding.CloseTimeout = TimeSpan.FromHours(1.0);
            //Binding.SendTimeout = TimeSpan.FromHours(1.0);
            //Binding.ReceiveTimeout = TimeSpan.FromHours(1.0);
            Binding.MaxReceivedMessageSize = 1024000;

            EndPoint = new EndpointAddress(url);
        }


        private IClientChannel _channel;
        public void Dispose() { }
        private IContract Client { get { return _channel as IContract; } }

        private bool _connected;
        private Task<bool> _connectTask;
        private bool Connect() {
            _channel = (IClientChannel)CreateChannel();
            try { _channel.Open(); } catch { return false; }
            return true;
        }
        private bool Connected {
            get {
                if (!_connected) {
                    if (_channel == null) _connected = Connect();
                    else {
                        if (_connectTask == null) _connectTask = Task.Factory.StartNew(() => Connect());
                        else
                        if (_connectTask.IsCompleted) {
                            _connected = _connectTask.Result;
                            _connectTask = null;
                        }
                    }
                }
                return _connected;
            }
        }

        protected virtual IContract CreateChannel() {
            ChannelFactory<IContract> factory = new ChannelFactory<IContract>(Binding, EndPoint);
            return factory.CreateChannel();
        }

        private object _Call(Func<IContract, object> operation) {
            if (Connected) {
                try { return operation(Client); } catch {
                    _connected = _channel.State == CommunicationState.Opened;
                    throw;
                }
            } 
            else throw new NacException("NotConnected");
        }

        public void Call(Action<IContract> action) {
            _Call((delegate (IContract client) {
                action(client);
                return null;
            }));
        }
        public object Call(Func<IContract, object> function) {
            return _Call((delegate (IContract client) {
                return function(client);
            }));
        }
    }
}

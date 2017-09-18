using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nac.Common {
    public abstract partial class NacLogger : IDisposable {
        private const int cDefaultFlushCount = 10;
        private const int cMaxQueueLenght = 100;

        private Thread _manager;
        private AutoResetEvent _newItem = new AutoResetEvent(false);
        private AutoResetEvent _dispose = new AutoResetEvent(false);
        private int _evt = 0;

        public NacLogger() {
            _manager = new Thread(new ThreadStart(() => {
                List<string> cachedMessages = new List<string>();
                string _prevMessage = null; uint _lastCount = 0;
                WaitHandle[] events = new WaitHandle[] { _newItem, _dispose };
                object item;
                do {
                    _evt = WaitHandle.WaitAny(events);
                    while (CachedItems.TryDequeue(out item)) {
                        var message = GetMessageFor(item);
                        if (message == _prevMessage) { ++_lastCount; if (!Disposing) continue; }
                        if (Devices.Any(device => device is IStorageDevice) && _prevMessage != null) {
                            if (_prevMessage != null) cachedMessages.Add($"~{DateTime.Now}[{_lastCount}]{Environment.NewLine}{_prevMessage}$");
                            if (Disposing) cachedMessages.Add($"{DateTime.Now}{Environment.NewLine}{message}");
                        }
                        _prevMessage = message; _lastCount = 0;
                        foreach (var device in Devices.OfType<IInteractiveDevice>()) device.Print(message);
                    }
                    if (cachedMessages.Count >= cDefaultFlushCount || Disposing) {
                        Flush(cachedMessages.ToArray(), Devices.OfType<IStorageDevice>());
                        cachedMessages.Clear();
                    }
                } while (!Disposing);
            }));
            _manager.Start();
        }

        ~NacLogger() { Dispose(); }
        public interface IDevice { }

        public interface IInteractiveDevice : IDevice {
            void Print(string message);
        }
        public interface IStorageDevice : IDevice {
            void Print(string[] messages);
        }

        protected virtual IEnumerable<IDevice> Devices { get { return null; } }

        private ConcurrentQueue<object> _cachedItems;
        private ConcurrentQueue<object> CachedItems {
            get {
                if (_cachedItems == null) _cachedItems = new ConcurrentQueue<object>();
                return _cachedItems;
            }
        }

        public bool Log(params object[] args) {
            if (Disposing) return false;
            Handle(args);
            return true;
        }

        private Task _handleTask;
        void Handle(params object[] args) {
            NacUtils.Decouple(() => {
                if (CachedItems.Count == cMaxQueueLenght) CachedItems.Enqueue("------Cache Full--------");
                if (CachedItems.Count > cMaxQueueLenght) return;
                CachedItems.Enqueue(args);
                _newItem.Set();

            }
            , ref _handleTask);
        }

        private string GetMessageFor(object item) {
            object[] args = (object[])item;
            return string.Join(Environment.NewLine, args.Select(arg => $"{{{Format(arg)}}}"));
        }

        private Task Flush(string[] messages, IEnumerable<IStorageDevice> devices) {
            return Task.Factory.StartNew(() => {
                Parallel.ForEach(devices, (device) => {
                    try { device.Print(messages); } catch (Exception x) { Log(x, device); }
                });
            });
        }

        private bool Disposing { get { return _evt > 0; } }

        public void Dispose() { // To be called as last in context
            CachedItems.Enqueue(new[] { "Disposing" });
            _dispose.Set();
            _manager.Join();
        }
    }

    #region Format
    public partial class NacLogger {
        private string Format(object arg) {
            string ret = "???";
            var pi = arg.GetType().GetProperty("StringToParse");

            if (pi != null) {
                ret = (string)pi.GetValue(arg);
            } else {
                try { ret = arg.ToString(); } catch { ret = $"ToString Exception for {nameof(arg)} {arg.GetType().Name}"; }
            }

            return ret;
        }
    }
    #endregion
}



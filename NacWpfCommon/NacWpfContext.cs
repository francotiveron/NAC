using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nac.Common;
using System.Collections.Specialized;
using Nac.Wpf.Common;
using System.Windows.Threading;
using System.Windows;
using System.Diagnostics;
using Nac.Wpf.UI;

public interface INacWpfContext : INacContext {}

public static class NacWpfGlobal {
    public static NacWpfContext G { get { return NacGlobal.G as NacWpfContext; } set { NacGlobal.G = value; } }
}

namespace Nac.Wpf.UI {
    using System.Text.RegularExpressions;
    using static NacWpfGlobal;

    public class NacWpfContext : NacWpfObjectWithChildren, INacWpfContext, ICollection<NacWpfEngine>/*, INotifyCollectionChanged*/ {
        private DispatcherTimer RefreshTimer { get; set; }
        private Dictionary<string, NacWpfEngine> _engines = new Dictionary<string, NacWpfEngine>();

        private NacWpfContext() {
            RefreshTimer = new DispatcherTimer();
            RefreshTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            RefreshTimer.Tick += RefreshTimer_Tick; ;
            RefreshTimer.Start();
        }

        private static NacWpfContext Instance { get { return G as NacWpfContext; } }

        public static void Create() {
            G = new NacWpfContext();
        }

        public static void Destroy() {
            Instance.Dispose();
        }

        long l1 = 0, l2 = 0;
        Stopwatch sw = new Stopwatch();
        private void RefreshTimer_Tick(object sender, EventArgs e) {
            try {
                Refreshing = true;
                var w = Application.Current.MainWindow;
                Predicate<object> IsRefreshable = obj => null != obj.GetType().GetMethod("NacWpfRefresh");
                sw.Restart();
                var refreshables = NacWpfUtils.Controls(w, IsRefreshable);
                sw.Stop();
                l1 = Math.Max(sw.ElapsedMilliseconds, l1);
                sw.Restart();
                foreach (var r in refreshables) {
                    var mi = r.GetType().GetMethod("NacWpfRefresh");
                    try {
                        mi.Invoke(r, null);
                    } catch (Exception x) when (Log(x, mi, r)) { }
                }
                sw.Stop();
                l2 = Math.Max(sw.ElapsedMilliseconds, l2);
            } finally { Refreshing = false; }
        }

        public void AddEngine(NacWpfEngine engine) {
            _engines.Add(engine.Host, engine);
            engine.NewEngineMessageEvent += Engine_NewEngineMessageEvent;
            OnNotifyChildrenCollectionChanged(NotifyCollectionChangedAction.Add, new[] { engine });
        }
        public void RemoveEngine(NacWpfEngine engine) {
            engine.NewEngineMessageEvent -= Engine_NewEngineMessageEvent;
            _engines.Remove(engine.Host);
            OnNotifyChildrenCollectionChanged(NotifyCollectionChangedAction.Remove, new[] { engine });
        }

        public event NewEngineMessageHandler NewEngineMessageEvent;
        private void Engine_NewEngineMessageEvent(string[] messages) {
            NewEngineMessageEvent?.Invoke(messages);
        }

        #region ICollection<NacWpfEngine>
        public new int Count {
            get {
                throw new NotImplementedException();
            }
        }

        public new bool IsReadOnly {
            get {
                throw new NotImplementedException();
            }
        }

        public void Add(NacWpfEngine item) {
            throw new NotImplementedException();
        }

        public new void Clear() {
            throw new NotImplementedException();
        }

        public bool Contains(NacWpfEngine item) {
            throw new NotImplementedException();
        }

        public void CopyTo(NacWpfEngine[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(NacWpfEngine item) {
            throw new NotImplementedException();
        }

        public new IEnumerator<NacWpfEngine> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _engines.Values.GetEnumerator();
        }

        #endregion

        #region INacWpfContext
        public NacWpfEngine GetEngine(string path) {
            Uri u = new Uri(path);
            return _engines[u.Host];
        }

        public bool Log(params object[] args) {
            return true;
        }

        //public void Dispose() {
        //    //throw new NotImplementedException();
        //}

        //public dynamic Engine {
        //    get {
        //        throw new NotImplementedException();
        //    }
        //}

        //public dynamic Field {
        //    get {
        //        throw new NotImplementedException();
        //    }
        //}

        bool _refreshing;
        public bool Refreshing { get { return _refreshing; } private set { _refreshing = value; } }
        #endregion

        //#region INotifyCollectionChanged
        //public event NotifyCollectionChangedEventHandler CollectionChanged;
        //#endregion
    }
}

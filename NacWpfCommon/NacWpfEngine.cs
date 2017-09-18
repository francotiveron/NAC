using Nac.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Nac.Common.Control;
using System.Collections.Specialized;
using Nac.Wpf.Common.Control;
using static NacUtils;
using static NacWpfUtils;
using Nac.Wcf.Common;
using System.Collections;
using System.Windows.Data;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace Nac.Wpf.Common {
    public class NacWpfEngine : NacWpfObjectWithChildren,/* ICollection<NacWpfObject>,*/ INacWpfChildrenOwner {
        private const string _offline = "offline";

        public NacWpfEngine(string host = _offline) : base(new NacObjectWithChildren()) {
            Name = Host = host;
            Path = $"net.tcp://{Host}:65456";
            Catalog = new NacCatalog<NacWpfObject>();
            Init();
        }

        public bool IsOffline { get { return Host == _offline; } }

        private NacWcfEngineClient _client;

        private NacWcfEngineClient Client {
            get {
                if (_client == null && !IsOffline) {
                    _client = new NacWcfEngineClient(Path);
                    _client.CallbackObject.NewEngineMessageEvent += _client_NewEngineMessageEvent;
                }
                return _client;
            }
        }

        public event NewEngineMessageHandler NewEngineMessageEvent;
        private void _client_NewEngineMessageEvent(string[] messages) {
            NewEngineMessageEvent?.Invoke(messages);
        }


        public IEnumerable<NacWpfObject> Projects { get { return Children; } }

        public string Host { get; set; }

        private bool ChangeAllowed {
            get {
                return NacSecurity.CanChangeRemoteEngines;
            }
        }

        private void Init() {
            using (new NacWpfWaitCursor()) {
                if (IsOffline) Catalog = new NacCatalog<NacWpfObject>();
                else {
                    NacCatalog<NacObject> catalog = null;
                    if (Succeeds(() => Client.GetCatalog(), ref catalog, warn: true))
                        Catalog = new NacCatalog<NacWpfObject>(catalog.Select(nacObject => Create(nacObject)));
                }
            }
        }


        //WPF propagation to containers is in this class on collection catalog methods
        public void Add(IEnumerable<NacObject> list) {
            if (IsOffline || (ChangeAllowed && Succeeds(() => Client.Add(list), warn: true))) {
                Add(list.Select(nacObject => Create(nacObject)).ToArray());
                NacSecurity.Log(Name, "Add");
            }
        }

        public void Add(NacObject obj) {
            Add(new List<NacObject> { obj });
        }

        //WPF propagation to containers is in this class on collection catalog methods
        public void Delete(string[] paths) {
            if (IsOffline || (ChangeAllowed && Succeeds(() => Client.Delete(paths), warn: true)))
                foreach (var path in paths) Remove(path);
                NacSecurity.Log(Name, "Delete");
        }

        public void Delete(string path) {
            Delete(new string[] { path });
        }

        public void Delete(NacWpfObject wpfObject) {
            Delete(wpfObject.Flat().Select(nacObject => nacObject.Path).ToArray());
        }

        //WPF propagation occurs in the property set accessor of the NacWpfObject wrapper
        public void Update(string path, string property, object value) {
            Update(new string[] { path }, property, new object[] { value });
        }
        public void Update(string[] paths, string property, object[] values) {
            if (IsOffline || (ChangeAllowed && Succeeds(() => Client.Update(paths, property, values.Select(v => new NacWcfObject(v)).ToArray()), warn: true))) {
                for (int i = 0; i < Math.Min(paths.Length, values.Length); i++)
                    NacUtils.Update(Catalog[paths[i]], property, values[i]);
                NacSecurity.Log(Name, "Update");
            }
        }

        //WPF propagation occurs in the caller
        public Dictionary<string, NacTagValue> GetValues(string[] tagPaths) {
            return Protect(() => Client?.GetValues(tagPaths));
        }

        public Dictionary<string, NacTagValue> GetRTDBValues(string rtdbPath) {
            return Protect(() => Client?.GetRDTBValues(rtdbPath));
        }

        public Dictionary<string, NacExecutionStatus> GetSectionStates(string sectionPath) {
            return Protect(() => Client?.GetSectionStates(sectionPath));
        }

        //WPF propagation occurs in the caller
        public void Connect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector) {
            if (IsOffline || (ChangeAllowed && Succeeds(() => Client.Connect(blockPathSource, blockPathDestination, sourceConnector, destinationConnector), warn: true))) {
                var src = this[blockPathSource].Base as NacBlockSeq;
                var dst = this[blockPathDestination].Base as NacBlockSeq;
                src.ConnectNext(dst.Path, sourceConnector);
                dst.ConnectPrev(src.Path, destinationConnector);
                NacSecurity.Log(Name, "Connect");
            }
        }

        public void Disconnect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector) {
            if (IsOffline || (ChangeAllowed && Succeeds(() => Client.Disconnect(blockPathSource, blockPathDestination, sourceConnector, destinationConnector), warn: true))) {
                var src = this[blockPathSource].Base as NacBlockSeq;
                var dst = this[blockPathDestination].Base as NacBlockSeq;
                src.DisconnectNext(dst.Path, sourceConnector);
                dst.DisconnectPrev(src.Path, destinationConnector);
                NacSecurity.Log(Name, "Disconnect");
            }
        }

        public void LoadProject() {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".nacp";
            dlg.Filter = "Nac projects (.nacp)|*.nacp";

            if (!IsOffline && !ChangeAllowed) return;

            var result = dlg.ShowDialog();

            if (result == true) {
                string filename = dlg.FileName;
                var nacObjects = NacSerializer.Deserialize(File.ReadAllText(filename), typeof(IEnumerable<NacObject>)) as IEnumerable<NacObject>;
                var newProjectPath = Guid.NewGuid().ToString();
                foreach (var nacObject in nacObjects) {
                    nacObject.Path = NacPath.Replace(nacObject.Path, Path, newProjectPath);
                    if (nacObject is NacProject) {
                        var project = nacObject as NacProject;
                        project.Database.Path = NacPath.Replace(project.Database.Path, Path, newProjectPath);
                    }
                    else
                    if (nacObject is NacBlockSeq) {
                        var blockSeq = nacObject as NacBlockSeq;
                        var nexts = blockSeq.Next.ToArray(); blockSeq.Next.Clear();
                        foreach (var next in nexts) blockSeq.Next.Add(NacPath.Replace(next, Path, newProjectPath));
                        var prevs = blockSeq.Prev.ToArray(); blockSeq.Prev.Clear();
                        foreach (var prev in prevs) blockSeq.Prev.Add(NacPath.Replace(prev, Path, newProjectPath));
                        if (nacObject is NacBlockIf) {
                            var blockIf = nacObject as NacBlockIf;
                            var nextTrue = blockIf.NextTrue.ToArray(); blockIf.NextTrue.Clear();
                            foreach (var next in nextTrue) blockIf.NextTrue.Add(NacPath.Replace(next, Path, newProjectPath));
                        }
                    }
                }

                Add(nacObjects);
            }
        }

        #region Catalog
        private static NacWpfObject Create(NacObject obj) {
            NacWpfObject ret = null;

            if (obj is NacBlockCall) ret = new NacWpfBlockCall(obj as NacBlockCall);
            else if (obj is NacBlockFuzzy) ret = new NacWpfBlockFuzzy(obj as NacBlockFuzzy);
            else if (obj is NacBlockIf) ret = new NacWpfBlockIf(obj as NacBlockIf);
            else if (obj is NacBlockTimer) ret = new NacWpfBlockTimer(obj as NacBlockTimer);
            else if (obj is NacBlockSeq) ret = new NacWpfBlockSeq(obj as NacBlockSeq);
            else if (obj is NacBlock) ret = new NacWpfBlock(obj as NacBlock);
            else if (obj is NacSection) ret = new NacWpfSection(obj as NacSection);
            else if (obj is NacTask) ret = new NacWpfTask(obj as NacTask);
            else if (obj is NacTag) ret = new NacWpfTag(obj as NacTag);
            else if (obj is NacProject) ret = new NacWpfProject(obj as NacProject);

            return ret;
        }
        //private Dictionary<string, NacWpfObject> Catalog { get; set; }
        private NacCatalog<NacWpfObject> Catalog { get; set; }

        public NacWpfObject this[string path] {
            get {
                var nacPath = NacPath.Parse(path);

                if (nacPath.IsRoot) return this;
                else if (nacPath.IsDatabase) {
                    NacWpfObject project;
                    Catalog.TryGetValue(nacPath.ProjectPath, out project);
                    return (project as NacWpfProject)?.Database;
                }
                NacWpfObject ret;
                return Catalog.TryGetValue(path, out ret) ? ret : null;
                //return Catalog[path];
            }
        }

        public void Move(NacWpfObject wpfObject, bool down = false) {
            int i, j;

            if (FindSiblingOf(wpfObject, out i, out j, down)) {
                Swap(i, j);
                GetParentOf(wpfObject)?.OnNotifyChildrenCollectionChanged(NotifyCollectionChangedAction.Move, new[] { wpfObject });
            }
        }

        private bool FindSiblingOf(NacWpfObject wpfObject, out int i, out int j, bool forward = false) {
            i = Catalog.IndexOf(wpfObject);
            var parent = NacPath.GetParentOf(wpfObject.Path);
            var type = wpfObject.GetType();
            
            if (forward) {
                var n = Catalog.Count;
                for (j = i + 1; j < n; j++)
                    if (Catalog[j].GetType().Equals(type) && NacPath.GetParentOf(Catalog[j].Base) == parent)
                        return true;
            } else {
                for (j = i - 1; j >= 0; j--) {
                    if (Catalog[j].GetType().Equals(type) && NacPath.GetParentOf(Catalog[j].Base) == parent)
                        return true;
                }
            }

            return false;
        }

        private void Swap(int ii, int jj) {
            int i = ii > jj ? ii : jj;
            int j = ii > jj ? jj : ii;
            var tmpi = Catalog[i]; var tmpj = Catalog[j];
            Catalog.RemoveAt(i); Catalog.RemoveAt(j);
            Catalog.Insert(j, tmpi); Catalog.Insert(i, tmpj);
        }

        public INacWpfChildrenOwner GetParentOf(NacWpfObject wpfObject) {
            var path = NacPath.Parse(wpfObject.Path);
            return this[path.Parent] as INacWpfChildrenOwner;

            //if (path.IsProject) return this;
            //if (path.IsTag) {
            //    var project = Catalog[path.ProjectPath] as NacWpfProject;
            //    return project?.Database;
            //}
        }

        public void Add(NacWpfObject[] wpfObjects) {
            Catalog.Add(wpfObjects);
            GetParentOf(wpfObjects[0])?.OnNotifyChildrenCollectionChanged(NotifyCollectionChangedAction.Add, wpfObjects);
        }

        private void Remove(string path) {
            var wpfObject = Catalog[path];
            Catalog.Remove(path);
            wpfObject.Dispose();
            GetParentOf(wpfObject)?.OnNotifyChildrenCollectionChanged(NotifyCollectionChangedAction.Remove, new[] { wpfObject });
        }

        public IEnumerable<NacWpfObject> ChildrenOf(string path) {
            return Catalog.Values.Where(wpfObject => NacPath.GetParentOf(wpfObject.Path) == path);
        }
        #endregion
    }
}

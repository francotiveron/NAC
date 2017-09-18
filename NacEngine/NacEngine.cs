using Nac.Common.Control;
using System;
using Nac.Common;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using static NacEngineGlobal;
using Nac.Engine.Control;
using System.Linq;
using static NacUtils;

namespace Nac.Engine {
    [DataContract]
    //[KnownType(typeof(NacEngineBase))]
    public class NacEngine/* : NacEngineObjectWithChildren*/ {
        private Thread Executor;
        private ConcurrentQueue<Task> PreTasks;
        private bool _dispose = false;

        [DataMember]
        private NacCatalog<NacEngineObject> Catalog { get; set; }
        //private Dictionary<string, NacEngineObject> Catalog { get; set; }

        //public new NacEngineBase Base {
        //    get {
        //        if (base.Base == null) base.Base = new NacEngineBase();
        //        return base.Base as NacEngineBase;
        //    }
        //}

        //public NacEngine(NacEngineBase nacEngine) : base(nacEngine) { Init(); }
        //public NacEngine() : this(new NacEngineBase()) { }
        public NacEngine() { Init(); }
        [OnDeserializing]
        private void OnDeserialize(StreamingContext ctx) { Init(); }

        private static NacEngineObject Create(NacObject nacObject) {
            NacEngineObject ret = null;
            if (nacObject is NacProject) ret = new NacEngineProject(nacObject as NacProject);
            else if (nacObject is NacTask) ret = new NacEngineTask(nacObject as NacTask);
            else if (nacObject is NacSection) ret = new NacEngineSection(nacObject as NacSection);
            else if (nacObject is NacBlockCall) ret = new NacEngineBlockCall(nacObject as NacBlockCall);
            else if (nacObject is NacBlockFuzzy) ret = new NacEngineBlockFuzzy(nacObject as NacBlockFuzzy);
            else if (nacObject is NacBlockIf) ret = new NacEngineBlockIf(nacObject as NacBlockIf);
            else if (nacObject is NacBlockTimer) ret = new NacEngineBlockTimer(nacObject as NacBlockTimer);
            else if (nacObject is NacBlockSeq) ret = new NacEngineBlockSeq(nacObject as NacBlockSeq);
            else if (nacObject is NacBlock) ret = new NacEngineBlock(nacObject as NacBlock);
            else if (nacObject is NacTag) ret = new NacEngineTag(nacObject as NacTag);
            return ret;
        }

        public NacEngineObject this[string path] {
            get {
                var nacPath = NacPath.Parse(path);

                if (nacPath.IsRoot) return null;
                else if (nacPath.IsDatabase) {
                    NacEngineObject project;
                    Catalog.TryGetValue(nacPath.ProjectPath, out project);
                    return (project as NacEngineProject)?.Database;
                }

                NacEngineObject ret;
                return Catalog.TryGetValue(path, out ret) ? ret : null;
                //return Catalog[path];
            }
        }
        public IEnumerable<NacEngineProject> Projects { get { return Catalog.Values.OfType<NacEngineProject>(); } }

        private void Init() {
            if (Catalog == null) Catalog = new NacCatalog<NacEngineObject>();
            PreTasks = new ConcurrentQueue<Task>();
            Executor = new Thread(Execute);
        }

        public void Start() { Executor.Start(); }

        internal void Dispose() {
            _dispose = true;
            Executor.Join();
        }

        private object Sync(Func<object> func) {
            using (var task = new Task<object>(func)) {
                PreTasks.Enqueue(task);
                return task.Result;
            }
        }
        private void Sync(Action action) {
            using (var task = new Task(action)) {
                PreTasks.Enqueue(task);
                task.Wait();
            }
        }

        internal NacCatalog<NacObject> GetCatalog() {
            return Sync(() => {
                //return Catalog.ToDictionary(p => p.Key, p => p.Value.Base);
                return new NacCatalog<NacObject>(Catalog.Select(engObject => engObject.Base));
            }) as NacCatalog<NacObject>;
        }

        internal void Update(string[] paths, string property, object[] values) {
            Sync(() => {
                for (int i = 0; i < Math.Min(paths.Length, values.Length); i++) {
                    var engObject = this[paths[i]];
                    NacUtils.Update(engObject.Base, property, values[i]);
                    engObject.OnBaseChanged(property);
                }
                //NacUtils.Update(this[paths[i]], property, values[i]);
            });
        }

        internal void Add(IEnumerable<NacObject> list) {
            Sync(() => {
                foreach (var nacObject in list) Catalog.Add(nacObject.Path, Create(nacObject));
                var parents =
                    list
                    .Select(nacObject => NacPath.GetParentOf(nacObject.Path))
                    .Distinct()
                    .Select(path => this[path])
                    .OfType<NacEngineObjectWithChildren>();

                foreach (var parent in parents) parent.OnChildrenChanged();
            });
        }

        internal void Delete(string[] paths) {
            Sync(() => {
                foreach (var path in paths) {
                    var engObject = Catalog[path];
                    Catalog.Remove(path);
                    engObject.Dispose();
                }
                var parents =
                    paths
                    .Select(path => NacPath.GetParentOf(path))
                    .Distinct()
                    .Select(path => this[path])
                    .OfType<NacEngineObjectWithChildren>();

                foreach (var parent in parents) parent.OnChildrenChanged();
            });
        }


        internal void Connect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector) {
            Sync(() => {
                var src = this[blockPathSource] as NacEngineBlockSeq;
                var dst = this[blockPathDestination] as NacEngineBlockSeq;
                if (src != null && dst != null) {
                    src.Base.ConnectNext(dst.Path, sourceConnector);
                    dst.Base.ConnectPrev(src.Path, destinationConnector);
                    var section = GetParentOf(src) as NacEngineObjectWithChildren;
                    section?.OnChildrenChanged();
                }
            });
        }

        internal void Disconnect(string blockPathSource, string blockPathDestination, string sourceConnector, string destinationConnector) {
            Sync(() => {
                var src = this[blockPathSource] as NacEngineBlockSeq;
                var dst = this[blockPathDestination] as NacEngineBlockSeq;
                if (src != null && dst != null) {
                    src.Base.DisconnectNext(dst.Path, sourceConnector);
                    dst.Base.DisconnectPrev(src.Path, destinationConnector);
                    var section = GetParentOf(src) as NacEngineObjectWithChildren;
                    section?.OnChildrenChanged();
                }
            });
        }

        internal Dictionary<string, NacTagValue> GetValues(string[] tagPaths) {
            return Sync(() => {
                //var q =
                //    from kv in Catalog
                //    join path in tagPaths
                //    on kv.Key equals path
                //    select new KeyValuePair<string, NacTagValue>(path, (kv.Value.Base as NacTag).Tag);

                var q =
                from engObject in Catalog
                join path in tagPaths
                on engObject.Path equals path
                select new KeyValuePair<string, NacTagValue>(path, (engObject.Base as NacTag).Tag);

                return q.ToDictionary(p => p.Key, p => p.Value);
            }) as Dictionary<string, NacTagValue>;
        }

        internal Dictionary<string, NacTagValue> GetRTDBValues(string rtdbPath) {
            var db = this[rtdbPath] as NacEngineDatabase;
            return GetValues(db.Tags.Select(tag => tag.Path).ToArray());
        }

        internal Dictionary<string, NacExecutionStatus> GetStates(string[] blockPaths) {
            return Sync(() => {
                var q =
                    from engObject in Catalog
                    join path in blockPaths
                    on engObject.Path equals path
                    select new KeyValuePair<string, NacExecutionStatus>(path, (engObject.Base as NacBlock).Status);

                return q.ToDictionary(p => p.Key, p => p.Value);
            }) as Dictionary<string, NacExecutionStatus>;
        }

        internal Dictionary<string, NacExecutionStatus> GetSectionStates(string sectionPath) {
            var section = this[sectionPath] as NacEngineSection;
            return GetStates(section.Blocks.Select(block => block.Path).ToArray());
        }

        private void Execute() {
            ulong cycleCounter = 0;
            while (!_dispose) {
                Thread.Sleep(100);
                ++cycleCounter;

                Task task;
                while (PreTasks.TryDequeue(out task))
                    try { task.RunSynchronously(); }
                    catch (Exception x) when (G.Log(x, task)) { }

                if ((cycleCounter % 10) != 0) continue;

                Parallel.ForEach(
                    Projects, project => {
                        try { project.Execute(); }
                        catch (Exception x) when (G.Log(x, project)) { }
                    }
                );
            }
        }
        public IEnumerable<NacEngineObject> ChildrenOf(string path) {
            return Catalog.Values.Where(engObject => NacPath.GetParentOf(engObject.Path) == path);
        }

        public object GetParentOf(NacEngineObject engObj) {
            return GetParentOf(engObj.Path);
        }

        //public object GetParentOf(string path) {
        //    return GetParentOf(NacPath.Parse(path));
        //}

        public object GetParentOf(NacPath path) {
            if (path.IsProject) return this;
            if (path.IsTag) {
                var prj = this[path.ProjectPath] as NacEngineProject;
                return prj.Database;
            }
            return this[path.Parent];
        }
        internal void Cleanup() {
            foreach (var block in Catalog.Values.OfType<NacEngineBlockSeq>()) {
                string[] paths = block.Base.Next.ToArray();
                foreach (var path in paths) if (!Catalog.Contains(path)) block.Base.Next.Remove(path);
                paths = block.Base.Prev.ToArray();
                foreach (var path in paths) if (!Catalog.Contains(path)) block.Base.Prev.Remove(path);
                if (block is NacEngineBlockIf) {
                    NacEngineBlockIf blockIf = block as NacEngineBlockIf;
                    paths = blockIf.Base.NextTrue.ToArray();
                    foreach (var path in paths) if (!Catalog.Contains(path)) blockIf.Base.NextTrue.Remove(path);

                }
            }
        }

    }
}
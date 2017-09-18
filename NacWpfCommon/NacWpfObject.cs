using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using static NacWpfGlobal;
using System.Collections;
using Nac.Common;
using Nac.Wpf.Common.Control;
using System.Windows.Data;
using System.Text.RegularExpressions;

namespace Nac.Wpf.Common {
    public abstract class NacWpfObject : NacObjectExtension<NacObject>, INotifyPropertyChanged {
        public NacWpfObject(NacObject o) : base(o) { }
        public NacWpfObject() { }

        public new string Path {
            get { return Base?.Path; }
            set { if (value != Base.Path) { Base.Path = value; OnNotifyPropertyChanged("Path"); } }
        }

        public NacWpfEngine Engine { get { return G.GetEngine(Path) as NacWpfEngine; } }
        //public IEnumerable<NacWpfObject> Children { get { return Engine.ChildrenOf(Path); } }


        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnNotifyPropertyChanged(string propName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public string DisplayName {
            get {
                NacWpfEngine engine = Engine;
                NacPath nacPath = Path;
                string ret = engine.Host;
                if (nacPath.ProjectPart != null) ret += NacPath.Separator + ((NacWpfNamedObject)engine[nacPath.ProjectPath]).Name;
                if (nacPath.IsDatabase) ret += NacPath.DBMark;
                if (nacPath.TagPart != null) ret += NacPath.DBMark + ((NacWpfNamedObject)engine[nacPath.TagPath]).Name;
                if (nacPath.TaskPart != null) ret += NacPath.Separator + ((NacWpfNamedObject)engine[nacPath.TaskPath]).Name;
                if (nacPath.SectionPart != null) ret += NacPath.Separator + ((NacWpfNamedObject)engine[nacPath.SectionPath]).Name;
                //if (nacPath.BlockPart != null) ret += NacPath.Separator + engine[nacPath.BlockPath].Name;

                return ret;
            }
        }

        public IEnumerable<NacObject> Flat() {
            List<NacObject> result = new List<NacObject>();
            _Flat(this, result);
            return result;
        }
        public static void _Flat(NacWpfObject wpfObject, List<NacObject> list) {
            if (!(wpfObject is NacWpfDatabase)) list.Add(wpfObject.Base);
            if (wpfObject is NacWpfObjectWithChildren) {
                if (wpfObject is NacWpfProject) _Flat(((NacWpfProject)wpfObject).Database, list);
                foreach (var child in ((NacWpfObjectWithChildren)wpfObject).Children)
                    _Flat(child, list);
            }
        }
        public bool IsSuccessorOf(NacWpfObject maybeAncestor) {
            return Regex.IsMatch(Path, $"^{maybeAncestor.Path}");
        }

        public override string ToString() {
            return DisplayName;
        }
    }

    public abstract class NacWpfNamedObject : NacWpfObject {
        public NacWpfNamedObject(NacObject o) : base(o) { }
        public NacWpfNamedObject() { }

        //public new string Name {
        //    get { return Base.Name; }
        //    set { if (value != Base.Name) { Base.Name = value; OnNotifyPropertyChanged("Name"); } }
        //}
        public string Name {
            get { return Base.Name; }
            set { if (value != Base.Name) { Base.Name = value; OnNotifyPropertyChanged("Name"); } }
        }
    }

    public interface INacWpfChildrenOwner: INotifyCollectionChanged {
        IEnumerable<NacWpfObject> Children { get; }
        void OnNotifyChildrenCollectionChanged(NotifyCollectionChangedAction action, IList items);
    }

    public abstract class NacWpfObjectWithChildren : NacWpfNamedObject, INacWpfChildrenOwner, ICollection<NacWpfObject> {
        public NacWpfObjectWithChildren(NacObjectWithChildren nac) : base(nac) { }
        public NacWpfObjectWithChildren() { }

        public virtual IEnumerable<NacWpfObject> Children { get { return Engine.ChildrenOf(Path); } }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public virtual void OnNotifyChildrenCollectionChanged(NotifyCollectionChangedAction action, IList items = null) {
            NotifyCollectionChangedEventHandler handlers = CollectionChanged;
            if (handlers != null) {
                foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList()) {
                    if (handler.Target is CollectionView)
                        ((CollectionView)handler.Target).Refresh();
                    else
                        handler(this, new NotifyCollectionChangedEventArgs(action, items));
                }
            }
        }

        #region ICollection<NacWpfObject>

        public int Count {
            get {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly {
            get {
                throw new NotImplementedException();
            }
        }
        public virtual void Add(NacWpfObject item) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public bool Contains(NacWpfObject item) {
            throw new NotImplementedException();
        }

        public void CopyTo(NacWpfObject[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(NacWpfObject item) {
            throw new NotImplementedException();
        }

        public IEnumerator<NacWpfObject> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Children.GetEnumerator();
        }

        #endregion

    }

}

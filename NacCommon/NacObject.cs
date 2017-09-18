using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections;
using Nac.Common.Fuzzy;
using System.Dynamic;

namespace Nac.Common {
    public interface INacObject<TPath> {
        TPath Path { get; set; }
    }
    [DataContract, Serializable]
    public abstract class NacObjectExtension: DynamicObject, IDisposable {
        public NacObjectExtension() { }
        ~NacObjectExtension() { Dispose(false); }
        public void Dispose() { Dispose(true); }
        public virtual void Dispose(bool disposing) { }
    }
    [DataContract, Serializable]
    public class NacObjectExtension<T> : NacObjectExtension, INacObject<string> where T: NacObject {
        [DataMember]
        public virtual T Base { get; set; }
        public NacObjectExtension(T t) { Base = t; }
        public NacObjectExtension() {}
        public string Path { get { return Base.Path; } set { Base.Path = value; } }
        //public string Name { get { return Base.Name; } set { Base.Name = value; } }
    }

    [DataContract, Serializable]
    public enum NacObjectClass {[EnumMember]Unknown, [EnumMember]NacRoot, [EnumMember]NacProject, [EnumMember]NacDatabase, [EnumMember]NacTag, [EnumMember]NacTask, [EnumMember]NacSection, [EnumMember]NacBlock }
    [DataContract, Serializable]
    [KnownType(typeof(NacProject))]
    [KnownType(typeof(NacTag))]
    [KnownType(typeof(NacTagScope))]
    [KnownType(typeof(NacTask))]
    [KnownType(typeof(NacSection))]
    [KnownType(typeof(NacBlock))]
    [KnownType(typeof(NacBlockSeq))]
    [KnownType(typeof(NacBlockTimer))]
    [KnownType(typeof(NacBlockIf))]
    [KnownType(typeof(NacBlockFuzzy))]
    [KnownType(typeof(NacBlockCall))]
    [KnownType(typeof(NacMembershipFunction))]
    [KnownType(typeof(NacTrapezoidalMembershipFunction))]
    [KnownType(typeof(NacFuzzySet))]
    [KnownType(typeof(HashSet<NacFuzzySet>))]
    [KnownType(typeof(HashSet<string>))]
    public class NacObject : INacObject<string>, IDisposable {
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public virtual string Name { get;  set; }

        //public NacEngineBase Engine { get { return (NacEngineBase)G.GetEngine(Path); } }
        //public virtual NacCatalog Catalog { get { return Engine.Catalog; } }

        //public NacObjectExtension Extension {get; set;}

        #region dispose
        public void Dispose() {
            //Extension?.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing) {
            if (disposing) {
                // get rid of managed resources
            }
            // get rid of unmanaged resources
        }

        // only if you use unmanaged resources directly in this
        //~NacObject() {
        //    Dispose(false);
        //}
        #endregion
    }

    [DataContract, Serializable]
    public class NacObjectWithChildren : NacObject, ICollection<NacObject> {
        //public virtual IEnumerable<NacObject> Children { get { return null; } }
        //public IEnumerable<NacObject> Children { get { return Engine.ChildrenOf(Path); } }

        //public virtual void OnChildrenChanged() {}

        //public NacObject Find(string name) {
        //    return Children.FirstOrDefault(t => t.Name == name);
        //}


        #region ICollection<NacProject>

        public int Count {
            get {
                return 0;// Children.Count();
            }
        }

        public bool IsReadOnly {
            get {
                throw new NotImplementedException();
            }
        }
        public virtual void Add(NacObject item) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public bool Contains(NacObject item) {
            throw new NotImplementedException();
        }

        public void CopyTo(NacObject[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(NacObject item) {
            throw new NotImplementedException();
        }

        public IEnumerator<NacObject> GetEnumerator() {
            return null;// Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return null;//Children.GetEnumerator();
        }

        #endregion
    }

    public abstract class NacObjectWithChildrenExtension<T> : NacObjectExtension<T> where T : NacObjectWithChildren {
        public NacObjectWithChildrenExtension(T obj) : base(obj) { }
        //public IEnumerable<NacObjectExtension> Children { get { return Engine.ChildrenOf(Path).Select(obj => obj.Extension); } }
    }

}

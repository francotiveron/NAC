using Nac.Common;
using Nac.Common.Control;
using Nac.Common.Fuzzy;
using Nac.Engine.Control;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static NacEngineGlobal;

namespace Nac.Engine {
    //public class NacEngineObject<T> : NacObjectExtension<T> where T : NacObject {
    //    public NacEngineObject(T nacObject) : base(nacObject) { }
    //    //public string Path { get { return Base.Path; } }
    //    public NacEngine Engine { get { return G.Engine; } }

    //}
    [DataContract]
    [KnownType(typeof(NacEngineProject))]
    [KnownType(typeof(NacEngineTag))]
    [KnownType(typeof(NacTagScope))]
    [KnownType(typeof(NacEngineTask))]
    [KnownType(typeof(NacEngineSection))]
    [KnownType(typeof(NacEngineBlock))]
    [KnownType(typeof(NacEngineBlockTimer))]
    [KnownType(typeof(NacEngineBlockSeq))]
    [KnownType(typeof(NacEngineBlockIf))]
    [KnownType(typeof(NacEngineDatabase))]
    [KnownType(typeof(NacEngineBlockFuzzy))]
    [KnownType(typeof(NacEngineBlockCall))]
    [KnownType(typeof(NacMembershipFunction))]
    [KnownType(typeof(NacTrapezoidalMembershipFunction))]
    [KnownType(typeof(NacFuzzySet))]
    [KnownType(typeof(HashSet<NacFuzzySet>))]
    [KnownType(typeof(HashSet<string>))]
    public class NacEngineObject : NacObjectExtension<NacObject> {
        public NacEngineObject(NacObject nacObject) : base(nacObject) { }
        public NacEngine Engine { get { return G.Engine; } }
        public string Name { get { return Base.Name; } }
        public NacEngineProject Project { get { return Engine[NacPath.Parse(Path).ProjectPath] as NacEngineProject; } }
        public virtual void OnBaseChanged(string property = null) { }
    }

    [DataContract]
    public class NacEngineObjectWithChildren : NacEngineObject {
        public NacEngineObjectWithChildren(NacObject nacObject) : base(nacObject) { }

        public IEnumerable<NacEngineObject> Children { get { return Engine.ChildrenOf(Path); } }
        public virtual void OnChildrenChanged() { }
    }

    //public class NacEngineObjectWithChildren : NacEngineObjectWithChildren<NacObjectWithChildren> {
    //    public NacEngineObjectWithChildren(NacObjectWithChildren nacObjectWithChildren) : base(nacObjectWithChildren) { }
    //}

}

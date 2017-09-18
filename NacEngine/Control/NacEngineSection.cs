using Nac.Common;
using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Nac.Engine.Control {
    [DataContract]
    public class NacEngineSection : NacEngineObjectWithChildren {
        [DataMember]
        public new NacSection Base { get { return base.Base as NacSection; } set { base.Base = value; } }
        public NacEngineSection(NacSection section) : base(section) { }
        public IEnumerable<NacEngineBlock> Blocks { get { return Children.Cast<NacEngineBlock>(); } }

        public bool ChildrenChanged { get; set; }

        public override void OnChildrenChanged() {
            ChildrenChanged = true;
            base.OnChildrenChanged();
        }

        public bool IsSubroutine { get { return Base.IsSubroutine; } }

        private class ReferentialBlock {
            public NacEngineBlockSeq This;
            public int Level;
            public HashSet<ReferentialBlock> Prev, Next;
            public ReferentialBlock(NacEngineBlockSeq block) {
                Level = -1;
                Prev = new HashSet<ReferentialBlock>();
                Next = new HashSet<ReferentialBlock>();
                This = block;
            }
            //public override int GetHashCode() {
            //    return 0;
            //}
            //public override bool Equals(object obj) {
            //    var other = (ReferentialBlock)obj;
            //    return This.Name == other.This.Name;
            //}
        }

        //private class BlockSet : HashSet<NacBlock> {
        //    public BlockSet(IEnumerable<NacBlock> blocks) : base(blocks) { }

        //    public static implicit operator ExtSet(BlockSet blockSet) {
        //        var extSet = new ExtSet();
        //    }
        //}

        private class RefSet : HashSet<ReferentialBlock> {
            public RefSet(IEnumerable<NacEngineBlockSeq> blocks) {
                var map = blocks.Select(block => new ReferentialBlock(block)).ToDictionary(refBlock => refBlock.This.Path);
                foreach (var refBlock in map.Values) {
                    var nexts = refBlock.This.OutputConnections.Select(path => map[path]);
                    foreach (var next in nexts) refBlock.Next.Add(next);
                    var prevs = refBlock.This.InputConnections.Select(path => map[path]);
                    foreach (var prev in prevs) refBlock.Prev.Add(prev);

                    //var refBlock = new ReferentialBlock(block);
                    //refBlock.Prev = new List<ReferentialBlock>(block.Prev.Select(path => new ReferentialBlock(block.Catalog[path] as NacBlockSeq)));
                    //refBlock.Next = new List<ReferentialBlock>(block.Next.Select(path => new ReferentialBlock(block.Catalog[path] as NacBlockSeq)));
                    Add(refBlock);
                }
                
            }
        }

        private class EngSet : HashSet<NacEngineBlock>, IComparable{
            public int Level = -1;

            public EngSet(IEnumerable<NacEngineBlock> collection) : base(collection) {
            }

            //public EngSet(IEnumerable<NacBlock> blocks) : base(blocks.Select(block => NacEngineFactory.Get(block) as NacEngineBlock)) { }

            public EngSet(IGrouping<int, ReferentialBlock> group) : this(group.Select(refBlock => refBlock.This)) { Level = group.Key; }

            public int CompareTo(object obj) {
                var other = (EngSet)obj;
                return Level - other.Level;
            }

            public void Execute() { Parallel.ForEach(this, engBlock => engBlock.Execute()); }
        }

        private class Network : HashSet<ReferentialBlock> { }

        private class Chain : List<EngSet> {
            public Chain(EngSet extSet) { Add(extSet); }
            public Chain(Network network) {
                //var levels = new HashSet<Tuple<int, NacBlockSeq>>();
                Compile(network);
            }
            private void Compile(Network network) {
                var starts = network.Where(refBlock => refBlock.Prev.Count == 0);
                foreach (var refBlock in starts) SetLevelsBy(refBlock);
                var levelGroups = network.GroupBy(refBlock => refBlock.Level);
                var extSetList = levelGroups.Select(grp => new EngSet(grp)).ToList();
                extSetList.Sort();
                AddRange(extSetList);
            }

            private void SetLevelsBy(ReferentialBlock refBlock, int level = 0) {
                refBlock.Level = Math.Max(refBlock.Level, level);
                foreach (var next in refBlock.Next) SetLevelsBy(next, level + 1);
            }

            public void Execute() {
                foreach (var set in this) set.Execute();
            }
        }

        private Chain[] _chains;

        private Chain[] Chains {
            get {
                if (ChildrenChanged || _chains == null) { Compile(); ChildrenChanged = false; }
                return _chains;
            }
        }

        private void Compile() {
            _chains = new Chain[] { }; //not null to avoid continuously recalling here
            var chains = new List<Chain>();
            var seqGroups = Blocks.GroupBy(block => block is NacEngineBlockSeq).ToDictionary(group => group.Key);
            if (seqGroups.ContainsKey(false)) chains.Add(new Chain(new EngSet(seqGroups[false])));
            if (seqGroups.ContainsKey(true)) {
                var refBlocks = new RefSet(seqGroups[true].Cast<NacEngineBlockSeq>());
                Network network;
                while (FindNetwork(refBlocks, out network)) chains.Add(new Chain(network));
            }
            _chains = chains.ToArray();
        }

        private bool FindNetwork(RefSet refBlocks, out Network newNetwork) {
            if (refBlocks.Count > 0) {
                Network network = new Network();
                WalkNetwork(refBlocks.First(), refBlocks, network);
                newNetwork = network;
                return true;
            }

            newNetwork = null;
            return false;
        }

        private void WalkNetwork(ReferentialBlock pivot, RefSet refBlocks, Network network) {
            network.Add(pivot);
            refBlocks.Remove(pivot);

            foreach (var refBlock in pivot.Prev.Union(pivot.Next))
                if (refBlocks.Contains(refBlock)) WalkNetwork(refBlock, refBlocks, network);

        }

        private int _token;
        public void Execute() {
            if (IsSubroutine) {
                if (_token == 1) foreach (var block in Blocks) block.Status = new NacExecutionStatus();
                else _token = 1;
            }
            else Call();
        }
        public void Call() {
            Parallel.ForEach(Chains, chain => chain.Execute());
            _token = 0;
        }

    }
}

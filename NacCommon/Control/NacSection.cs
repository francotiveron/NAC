using Nac.Common.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Nac.Common.Control {
    [DataContract, Serializable]
    public class NacSection : NacObjectWithChildren {
        //public IEnumerable<NacBlock> Blocks { get { return Children.Cast<NacBlock>(); } }

        [DataMember]
        public bool IsSubroutine { get; set; }

        //public override void OnChildrenChanged() {
        //    ChildrenChanged = true;
        //    base.OnChildrenChanged();
        //}

        //public bool ChildrenChanged { get; set; }
    }


    /*
    public class NacSection : NacObjectWithChildren {
        public IEnumerable<NacBlock> Blocks { get { return Children.Cast<NacBlock>(); } }

        private class BlockSet : HashSet<NacBlock> {
            public BlockSet(IEnumerable<NacBlock> blocks) : base(blocks) { }
            public BlockSet() { }

            public void Execute() {
                Parallel.ForEach(this, block => block.Execute());
            }
        }
        private class Network : HashSet<NacBlockSeq> { }

        private class Chain : List<BlockSet> {
            public Chain(BlockSet blocks) { Add(blocks); }
            public Chain(Network network) {
                var levels = new HashSet<Tuple<int, NacBlockSeq>>();
            }
            private void ClassifyBlocks(Network network, HashSet<Tuple<int, NacBlockSeq>> levels, NacBlockSeq block = null) {

            }
            public void Execute() {
                foreach (var set in this) set.Execute();
            }
        }

        private Chain[] _chains;
        private Chain[] Chains {
            get {
                if (_chains == null) CompileToChains();
                return _chains;
            }
        }


        public override void OnChildrenChanged() {
            CompileToChains();
            base.OnChildrenChanged();
        }

        private void CompileToChains() {
            _chains = new Chain[] { }; //not null to avoid continuously recalling here
            var chains = new List<Chain>();
            var seqGroups = Blocks.GroupBy(block => block is NacBlockSeq).ToDictionary(group => group.Key);
            if (seqGroups.ContainsKey(false)) chains.Add(new Chain(new BlockSet(seqGroups[false])));
            if (seqGroups.ContainsKey(true)) {
                var seqBlocks = new BlockSet(seqGroups[true]);
                Network network;
                while (FindNetwork(seqBlocks, out network)) chains.Add(new Chain(network));
                _chains = chains.ToArray();
            }
        }

        private bool FindNetwork(BlockSet seqBlocks, out Network newNetwork) {
            if (seqBlocks.Count > 0) {
                Network network = new Network();
                WalkNetwork(seqBlocks.First() as NacBlockSeq, seqBlocks, network);
                newNetwork = network;
                return true;
            }

            newNetwork = null;
            return false;
        }

        private void WalkNetwork(NacBlockSeq pivot, BlockSet seqBlocks, Network network) {
            network.Add(pivot);
            seqBlocks.Remove(pivot);
            foreach (var path in (pivot.Prev.Union(pivot.Next))) {
                NacBlockSeq block = Catalog[path] as NacBlockSeq;
                if (seqBlocks.Contains(block)) WalkNetwork(block, seqBlocks, network);
            }
        }

        //public void Execute() { Parallel.ForEach(Chains, chain => chain.Execute()); }

    }
    */




}
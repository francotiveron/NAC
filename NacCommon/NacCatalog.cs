using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common {
    public class NacDictionary<TKey, TItem> : KeyedCollection<TKey, TItem> {
        protected override TKey GetKeyForItem(TItem item) {
            return default(TKey);
        }
        public NacDictionary() : base() { }

        public NacDictionary(IEnumerable<TItem> items) : this() {
            foreach (var item in items) Add(item);
        }
        public IEnumerable<TItem> Values { get { return Items; } }
        public void Add(string key, TItem item) { Add(item); }
        public void Add(TItem[] items) { foreach (TItem item in items) Add(item); }

        public bool TryGetValue(TKey key, out TItem item) {
            return Dictionary.TryGetValue(key, out item);
        }
    }
    public class NacCatalog<TItem> : NacDictionary<string, TItem> where TItem : INacObject<string> {
        protected override string GetKeyForItem(TItem item) {
            return item.Path;
        }
        public NacCatalog() : base() { }

        public NacCatalog(IEnumerable<TItem> items) : this() {
            foreach (var item in items) Add(item);
        }
    }
}

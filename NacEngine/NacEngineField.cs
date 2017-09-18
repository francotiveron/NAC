using System.ServiceModel;
using Nac.Common;
using System.Linq;
using System.Text.RegularExpressions;
using Nac.Common.Control;
using System.Collections.Generic;
using Nac.Wcf.Common;
using System;

namespace Nac.Engine {
    public class NacEngineField {
        private Dictionary<string, NacWcfFieldClient> _datasources = new Dictionary<string, NacWcfFieldClient>();

        public NacEngineField(string ip) {
            var url = $@"net.tcp://{ip}:65457";
            _datasources.Add(url, new NacWcfFieldClient(url));
        }
        ~NacEngineField() {
            _datasources.Values.ToList().ForEach(channel => channel.Dispose());
        }

        private void ResolveAddress(string address, out NacWcfFieldClient field, out string tagName) {
            string[] s = Regex.Split(address, @"(/)");
            int n = s.Length;
            tagName = s[n - 1];
            string url = string.Join(string.Empty, s.Take(n - 2));
            field = _datasources[url];
        }

        //public bool Write(string address, NacTagValue tagValue) {
        //    NacWcfFieldClient dataSource;
        //    string tagName;

        //    ResolveAddress(address, out dataSource, out tagName);
        //    return dataSource.Write(tagName, tagValue);
        //}
        public bool[] Write(string[] addresses, NacTagValue[] tagValues) {
            if (addresses.Length > 0) {
                return 
                    addresses
                    .Zip(tagValues, (address, value) => new { address, value })
                    .Select(av => {
                        NacWcfFieldClient dataSource;
                        string tagName;

                        ResolveAddress(av.address, out dataSource, out tagName);
                        return new { dataSource, tagName, av.value };
                    })
                    .GroupBy(obj => obj.dataSource, obj => new { obj.tagName, obj.value })
                    .SelectMany(group => {
                        var dataSource = group.Key;
                        return dataSource.Write(group.Select(a => a.tagName).ToArray(), group.Select(a => a.value).ToArray());
                    }).ToArray();
            }
            return null;
        }
        public NacTagValue[] Read(string[] addresses) {
            if (addresses.Length > 0) {
                return 
                    addresses
                    .Select(address => {
                        NacWcfFieldClient dataSource;
                        string tagName;

                        ResolveAddress(address, out dataSource, out tagName);
                        return new { dataSource, tagName };
                    })
                    .GroupBy(obj => obj.dataSource, obj => obj.tagName)
                    .SelectMany(group => {
                        var dataSource = group.Key;
                        return dataSource.Read(group.ToArray());
                    }).ToArray();
            }
            return null;
        }
    }
}

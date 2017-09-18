using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Nac.Field.Citect.CtApi;

namespace Nac.Field.Citect {
    public partial class NacFieldCitectProxy : IDisposable {
        private class CitectServer {
            internal string _address, _user, _password;
        }

        private CitectServer[] _servers;
        private CitectServer[] CitectServers {
            get {
                if (_servers == null) {
                    var citectServers = CitectServersConfigurationSection.Config.SettingsList.ToList();
                    _servers = citectServers
                    .Select(server => new CitectServer {
                        _address = server.Machine
                        , _user = server.Username
                        , _password = server.Password
                    }).ToArray();
                }
                return _servers;
            }
        }

        private int _currentServer = 0;
        private uint _hCtApi = 0;

        public NacFieldCitectProxy() { }

        NacFieldCitectTagList _citectList;
        private NacFieldCitectTagList CitectList {
            get {
                if (_citectList == null) {
                    _citectList = new NacFieldCitectTagList(CtApi);
                }
                return _citectList;
            }
       }

        public void Dispose() {
            _citectList?.Dispose();
            _citectList = null;
            if (_hCtApi != 0) ctClose(_hCtApi);
            _hCtApi = 0;
        }


        ~NacFieldCitectProxy() {
        }

        private uint CtApi {
            get {
                if (_hCtApi == 0)
                    _hCtApi = ctOpen(
                        CitectServers[_currentServer]._address,
                        CitectServers[_currentServer]._user,
                        CitectServers[_currentServer]._password
                        , 0/*, CT_OPEN_RECONNECT*/
                    );
                return _hCtApi;
            }
        }

        //public bool Read(string name, out string sValue) {
        //    StringBuilder sb = new StringBuilder(100);

        //    bool readOK = Redund(() => ctTagRead(CtApi, name, sb, sb.Capacity));
        //    sValue = readOK ? sb.ToString() : "?";
        //    return readOK;
        //}

        //public bool Read(string[] names, out string[] sValues) {
        //    bool readOK = false;

        //    sValues = names.Select(name => {
        //        string sValue;
        //        readOK |= Read(name, out sValue);
        //        return sValue;
        //    }).ToArray();

        //    return readOK;
        //}

        //public bool Read(string[] names, out string[] sValues) {
        //    var hList = ctListNew(Conn, 0/*, CT_LIST_EVENT_NEW | CT_LIST_EVENT_STATUS*/);
        //    var hTags = names.Foreach(name => ctListAdd(hList, name)).ToArray();
        //    //var hTags = names.Foreach(name => ctListAddEx(hList, name, true, 1000, 0.0));

        //    bool readOK = ctListRead(hList, 0);
        //    if (!readOK) {
        //        int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
        //        Debug.Print($"ctListRead={error}");
        //    }

        //    if (readOK) {
        //        StringBuilder sb = new StringBuilder(100);
        //        int n = hTags.Length;
        //        sValues = new string[n];
        //        for (int i = 0; i < n; i++)
        //            if (ctListData(hTags[i], sb, sb.Capacity, 0))
        //                sValues[i] = sb.ToString();
        //            else
        //                sValues[i] = "?";
        //    } else sValues = null;

        //    ctListFree(hList);

        //    return readOK;
        //}

        public bool Read(string[] names, out string[] sValues) {
            string[] values = null;
            bool readOK = Redund(() => {                
                CitectList.Read(names, out values);
                return values != null;
            });
            sValues = values;
            return readOK;
        }

        public bool Write(string[] names, string[] sValues, out bool[] successes) {
            var n = names.Length;
            var succs = new bool[n];

            bool writeOK = Redund(() => {
                return CitectList.Write(names, sValues, ref succs);
            });
            successes = succs;
            return writeOK;
        }
        //public bool Write(string[] names, string[] sValues, out bool[] successes) {
        //    var n = names.Length;
        //    var succs = new bool[n];
        //    bool writeOk =  Redund(() => {
        //        bool ret = false;

        //        for (int i = 0; i < n; i++)
        //            ret |= succs[i] = ctTagWrite(CtApi, names[i], sValues[i]);

        //        return ret;
        //    });

        //    successes = succs;
        //    return writeOk;
        //}

        private string[] _citectTypes = {
            "Digital", "Integer", "Real", "BCD", "Long", "Long BCD", "Long Real", "String", "Byte", "Void",
            "Unsigned integer", "???", "???", "???", "???", "???", "???", "???", "Unsigned Long", "???" };

        public string[] Browse(string filter) {
            uint hObj = 0;
            uint hFind = Redund(() => ctFindFirst(CtApi, "TAG", filter, ref hObj, 0));
            List<string> result = new List<string>();

            if (hFind != 0 && hObj != 0) {
                int len = 256, nTags = 0;
                StringBuilder sb = new StringBuilder(len);

                do {
                    if (ctGetProperty(hObj, "TAG", sb, sb.Capacity, ref len, DBTYPEENUM.DBTYPE_STR)) {
                        var name = sb.ToString();
                        ctGetProperty(hObj, "TYPE", sb, sb.Capacity, ref len, DBTYPEENUM.DBTYPE_STR);
                        int typeIndex; bool indexValid = int.TryParse(sb.ToString(), out typeIndex);
                        var type = indexValid && typeIndex >= 0 && typeIndex <= _citectTypes.Length ? _citectTypes[typeIndex] : "???";
                        ctGetProperty(hObj, "COMMENT", sb, sb.Capacity, ref len, DBTYPEENUM.DBTYPE_STR);
                        var comment = sb.ToString();
                        result.Add($"{name},{type},{comment}");
                        ++nTags;
                    }
                }
                while (ctFindNext(hFind, ref hObj) && nTags < 1000);
                ctFindClose(hFind);
            }
            return result.ToArray();
        }

    }

    //Redundancy
    public partial class NacFieldCitectProxy {

        private bool Redund(Func<bool> func) {
            var _firstServer = _currentServer;

            do {
                if (func()) return true;
                Dispose();
                _currentServer = (_currentServer + 1) % _servers.Length;

            } while (_currentServer != _firstServer);

            return false;
        }

        private uint Redund(Func<uint> func) {
            var _firstServer = _currentServer;

            do {
                uint ret = func();
                if (ret != 0) return ret;
                Dispose();
                _currentServer = (_currentServer + 1) % _servers.Length;

            } while (_currentServer != _firstServer);

            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nac.Field.Citect.CtApi;

namespace Nac.Field.Citect {
    public class NacFieldCitectTagList : IDisposable {
        private const uint cCitectErr25 = 268443673; //0x10002019 => 0x19 = Citect generic error 25 data not yet valid (to be ignored as per citect doc)
        private const uint cWinErr997 = 997; //0x3E5 => overlaped operation pending
        private uint PollTime;
        private bool Raw;
        private Dictionary<string, uint> tagHandlesRead = new Dictionary<string, uint>();
        private Dictionary<string, uint> tagHandlesWrite = new Dictionary<string, uint>();
        private Dictionary<string, string> tagCache = new Dictionary<string, string>();
        private uint CtApi;

        private uint HandleRead, HandleWrite;

        public NacFieldCitectTagList(uint hCtApi, uint pollTime = 500, bool raw = true) {
            HandleRead = ctListNew(hCtApi, 0/*, CT_LIST_EVENT_NEW | CT_LIST_EVENT_STATUS*/);
            //HandleWrite = ctListNew(hCtApi, 0/*, CT_LIST_EVENT_NEW | CT_LIST_EVENT_STATUS*/);
            CtApi = hCtApi;
            PollTime = pollTime;
            Raw = raw;
        }

        private void UpdateRead(string[] tagNames) {
            foreach (var tagName in tagNames)
                if (!tagHandlesRead.ContainsKey(tagName)) {
                    tagHandlesRead.Add(tagName, ctListAddEx(HandleRead, tagName, Raw, PollTime, 0d));
                    tagCache[tagName] = "?";
                }
        }

        private void UpdateWrite(string[] tagNames) {
            foreach (var tagName in tagNames)
                if (!tagHandlesWrite.ContainsKey(tagName))
                    tagHandlesWrite.Add(tagName, ctListAddEx(HandleWrite, tagName, Raw, PollTime, 0d));
        }

        public void Dispose() { ctListFree(HandleRead); HandleRead = 0; ctListFree(HandleWrite); HandleWrite = 0; }

        internal bool Read(string[] names, out string[] sValues) {
            UpdateRead(names);
            int n = names.Length;
            sValues = new string[n];

            if (ctListRead(HandleRead, 0)) {
                StringBuilder sb = new StringBuilder(255);

                for (int i = 0; i < n; i++)
                    if (ctListData(tagHandlesRead[names[i]], sb, sb.Capacity, 0))
                        tagCache[names[i]] = sValues[i] = sb.ToString();
                    else {
                        int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                        if (error == cCitectErr25) sValues[i] = tagCache[names[i]];
                        else sValues[i] = "?";
                        //Debug.Print($"ctListRead={error}");
                    }
            } else {
                int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                if (error == cWinErr997)
                    for (int i = 0; i < n; i++) sValues[i] = tagCache[names[i]];
                else sValues = null;
            }

            return sValues != null;
        }

        public bool Write(string[] names, string[] sValues, ref bool[] successes) {
            var n = names.Length;

            bool writeOK = false;

            for (int i = 0; i < n; i++)
                if (ctTagWrite(CtApi, names[i], sValues[i])) {
                    writeOK |= successes[i] = true;
                    tagCache[names[i]] = sValues[i];
                }

            return writeOK;
        }
        internal bool Write1(string[] names, string[] sValues, out bool[] successes) {
            //successes = null;
            //return false;
            UpdateWrite(names);
            int n = names.Length;
            successes = new bool[n];
            //for (int i = 0; i < n; i++) successes[i] = ctListWrite(tagHandlesWrite[names[i]], sValues[i], 0);
            if (!ctListWrite(tagHandlesWrite[names[0]], sValues[0], 0)) {
                int error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                Debug.Print($"ctListRead={error}");
            }

            return true;
            //return successes.Any(success => success);
        }
    }
}

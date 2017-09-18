using Citect.Util;
using System;
using System.Collections.Generic;
using System.Text;
using static Citect.Util.CTAPI;

namespace CtApiTest {
    class Program {
        static void Main(string[] args) {

            uint hCtapi = ctOpen("203.15.179.7", "Administrator", "alfa1", 0);
            if (hCtapi == 0) {
                Console.Out.WriteLine("Could not open CTAPI connection");
                return;
            }

            StringBuilder sb = new StringBuilder(100);
            var hList = ctListNew(hCtapi, 0/*, CT_LIST_EVENT_NEW | CT_LIST_EVENT_STATUS*/);
            var hTag = ctListAdd(hList, "SCS_122WT002_AT_PV");
            bool readOK = ctListRead(hList, 0);
            bool tagOK = ctListData(hTag, sb, sb.Capacity, 0);

            ctListFree(hList);
            ctClose(hCtapi);












            //uint hCtapi = CTAPI.ctOpen(null, "Administrator", "alfa1", 0);
            //if (hCtapi == 0) {
            //    Console.Out.WriteLine("Could not open CTAPI connection");
            //    return;
            //}

            //bool writeOK = CTAPI.ctTagWrite(hCtapi, "NoC2A", "1");
            //if (!writeOK) {
            //    Console.Out.WriteLine("TagWrite failed");
            //}

            //CTAPI.ctClose(hCtapi);
        }
    }
}

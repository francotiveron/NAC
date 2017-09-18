using Nac.Common;
using Nac.Common.Control;
using Nac.Common.Control.Code;
using Nac.Common.Control.Code.Block;
using Nac.Common.Control.Data;
using Nac.Common.ServiceContracts;
using Nac.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceClientTest {
    class Program {
        static INacWcf engine = null;
        static INacFieldXch citect = null;

        static void Main(string[] args) {
            EngineTests();
            //CitectTests();
            Console.ReadLine();
        }
        private static void EngineTests() {
#if SERVICE
            var myBinding = new NetTcpBinding();
            var myEndpoint = new EndpointAddress("net.tcp://localhost:65456");
            var myChannelFactory = new ChannelFactory<INacWcf>(myBinding, myEndpoint);
            engine = myChannelFactory.CreateChannel();
#else
            NacEngEngine.Start();
            engine = new NacEngEngine();
#endif
            try {
                EngineTest4();

#if SERVICE
                ((ICommunicationObject)engine).Close();
#endif
            }
            catch (Exception x) {
#if SERVICE
                if (engine != null) {
                    ((ICommunicationObject)engine).Abort();
                }
#endif
            }

        }
        private static void CitectTests() {
#if SERVICE
            var myBinding = new NetTcpBinding();
            var myEndpoint = new EndpointAddress("net.tcp://localhost:65457");
            var myChannelFactory = new ChannelFactory<INacFieldXch>(myBinding, myEndpoint);
            citect = myChannelFactory.CreateChannel();
#else
            NacEngEngine.Start();
            engine = new NacEngEngine();
#endif
            try {
                CitectTest1();

#if SERVICE
                ((ICommunicationObject)engine).Close();
#endif
            }
            catch (Exception x) {
#if SERVICE
                if (engine != null) {
                    ((ICommunicationObject)engine).Abort();
                }
#endif
            }

        }

        private static void EngineTest4() {
            NacProject project = new NacProject() { Path = "P1" };
            project.RTDB.Path = "P1//";
            engine.Checkin(project);

            NacTag t1 = new NacTag() { Path = "P1//T1", Scope = NacTagScope.Input, Address = @"net.tcp://localhost:65457/NoC2A" }; engine.Checkin(t1);
            NacTag t2 = new NacTag() { Path = "P1//T2" }; engine.Checkin(t2);
            NacTag t3 = new NacTag() { Path = "P1//T3" }; engine.Checkin(t3);
            engine.SetValue("P1//T1", new decimal(1));
            engine.SetValue("P1//T2", new decimal(2));
            NacTask task = new NacTask() { Path = "P1/T1" }; engine.Checkin(task);
            NacSection section = new NacSection() { Path = "P1/T1/S1" }; engine.Checkin(section);
            NacBlock block = new NacBlock() { Path = "P1/T1/S1/B1" };
            block.Code = "T3 = T2 + T1";
            engine.Checkin(block);
            Console.ReadLine();
            NacCatalog catalog = engine.GetCatalog();
        }

        static private void CitectTest1() {
            NacValueType v = citect.GetTagVal("NoC2A");
        }
    }
}

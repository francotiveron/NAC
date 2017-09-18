using Nac.Engine.Control;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NacEngineGlobal;

namespace Nac.Engine {
    public class NacEngineRuntime : DynamicObject {
        public NacEngineProject Project { get; private set; }
        public NacEngineRuntime(NacEngineProject project) { Project = project; }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            return Project.Database.TryGetMember(binder, out result);
            //try {
            //    result = Tags.First(x => x.Name == binder.Name);
            //    return true;
            //} catch (Exception x) when (G.Log(x, binder)) { result = null; }
            //return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value) {
            return Project.Database.TrySetMember(binder, value);
            //try {
            //    var tag = Tags.First(x => x.Name == binder.Name);
            //    tag.Tag = new NacTagValue(value);
            //    return true;
            //} catch (Exception x) when (G.Log(x, binder)) { }
            //return false;
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result) {
            return base.TryBinaryOperation(binder, arg, out result);
        }
        public override bool TryConvert(ConvertBinder binder, out object result) {
            return base.TryConvert(binder, out result);
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {
            return base.TryGetIndex(binder, indexes, out result);
        }
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value) {
            return base.TrySetIndex(binder, indexes, value);
        }
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result) {
            return base.TryInvoke(binder, args, out result);
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            return base.TryInvokeMember(binder, args, out result);
        }
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result) {
            return base.TryUnaryOperation(binder, out result);
        }
    }
}

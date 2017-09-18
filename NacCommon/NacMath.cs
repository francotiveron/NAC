using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Common {
    public class NacMath :  DynamicObject {
        //public static NacTagValue Log(NacTagValue tagVal) {
        //    return new NacTagValue(Math.Log(tagVal.Value));
        //}
        private Type _math;
        private Dictionary<string, MethodInfo> _cache;
        public NacMath() {
            _math = typeof(Math);
            _cache = new Dictionary<string, MethodInfo>();

        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
            MethodInfo method = null;
            if (!_cache.TryGetValue(binder.Name, out method)) {
                method = 
                _math
                .GetMethods()
                .Where(mi => mi.Name == binder.Name)
                .Where(mi => {
                    var pars = mi.GetParameters();
                    if (pars.Length != args.Length) return false;
                    return pars.All(par => par.ParameterType.Equals(typeof(double)));
                })
                .FirstOrDefault();

                if (method != null) _cache.Add(method.Name, method);
            }
            result = method?.Invoke(
                null
                , args.Select(arg => {
                    //if (arg is DynamicObject) return ((dynamic)arg).Value;
                    if (arg is DynamicObject) {
                        object ret = null;
                        using (dynamic darg = (dynamic)arg) {
                            ret = darg.Value;
                        }
                        return ret;
                    }
                    else
                    if (arg is NacTagValue) return ((NacTagValue)arg).Value;
                    return arg;
                  }).ToArray()
            );
            return method != null;
        }

        //public NacTagValue Avg(NacTagValue value, int n) {

        //}
    }
}

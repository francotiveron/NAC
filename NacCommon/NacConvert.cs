using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Nac.Common {
    public static class NacConvert {

        public static T To<T>(object obj) {
            if (obj is T || obj.GetType().IsCastableTo(typeof(T))) return (T)obj;
            else {
                var tryParse = 
                    typeof(T)
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(method => method.Name == "TryParse")
                    .Where(method => method.GetParameters().Count() == 2)
                    .First();

                if (tryParse != null && obj.GetType().IsCastableTo(typeof(string))) {
                    var str = (string)obj;
                    var pars = new object[] { str, null };
                    if ((bool)tryParse.Invoke(null, pars)) return (T)pars[1];
                }
            }
            return default(T);
        }
    }
}

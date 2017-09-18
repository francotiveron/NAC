using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nac.Common {
    public class NacClipboard<T> where T : class {
        public static T Get() {
            T retrievedObj = null;
            IDataObject dataObj = Clipboard.GetDataObject();
            string format = typeof(T).FullName;
            if (dataObj.GetDataPresent(format)) {
                retrievedObj = dataObj.GetData(format) as T;
            }
            return retrievedObj;
        }

        public static void Put(T what) {
            // register my custom data format with Windows or get it if it's already registered
            DataFormat format = DataFormats.GetDataFormat(typeof(T).FullName);

            // now copy to clipboard
            IDataObject dataObj = new DataObject();
            dataObj.SetData(format.Name, what, false);
            Clipboard.SetDataObject(dataObj, false);
        }

        public static void Clear() {
            Clipboard.Clear();
        }
    }
}

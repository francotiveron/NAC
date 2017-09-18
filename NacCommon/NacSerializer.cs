using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Nac.Common {
    public class NacSerializer {
        public static string Serialize(object value) {

            if (value == null) {
                return null;
            }

            DataContractSerializer serializer = new DataContractSerializer(value.GetType());

            using (StringWriter textWriter = new StringWriter()) {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter)) {
                    serializer.WriteObject(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        public static object Deserialize(string xml, Type type) {

            if (string.IsNullOrEmpty(xml)) {
                return default(object);
            }

            DataContractSerializer serializer = new DataContractSerializer(type);

            using (StringReader textReader = new StringReader(xml)) {
                using (XmlReader xmlReader = XmlReader.Create(textReader)) {
                    return serializer.ReadObject(xmlReader);
                }
            }
        }

    }
}

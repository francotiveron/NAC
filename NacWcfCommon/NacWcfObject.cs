using Nac.Common.Control;
using Nac.Common.Fuzzy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nac.Wcf.Common {
    [DataContract]
    [KnownType(typeof(Point))]
    [KnownType(typeof(NacTagScope))]
    [KnownType(typeof(NacTagValue))]
    [KnownType(typeof(NacValueQuality))]
    [KnownType(typeof(NacExecutionStatus))]
    [KnownType(typeof(NacExecutionQuality))]
    [KnownType(typeof(NacMembershipFunction))]
    [KnownType(typeof(NacTrapezoidalMembershipFunction))]
    [KnownType(typeof(NacFuzzySet))]
    [KnownType(typeof(HashSet<NacFuzzySet>))]
    [KnownType(typeof(HashSet<string>))]
    public class NacWcfObject: IConvertible {
        [DataMember]
        public object Content;

        public NacWcfObject(object content) { Content = content; }

        public TypeCode GetTypeCode() {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider) {
            return (bool)Content;
        }

        public byte ToByte(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider) {
            return (DateTime)Content;
        }

        public decimal ToDecimal(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider) {
            return (double)Content;
        }

        public short ToInt16(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider) {
            return Content as string;
        }

        public object ToType(Type conversionType, IFormatProvider provider) {
            return Content;
        }

        public ushort ToUInt16(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public static implicit operator NacTagScope(NacWcfObject obj) { return (obj.Content as NacTagScope?).Value; }
        public static implicit operator NacWcfObject(NacTagScope obj) { return new NacWcfObject(new NacTagScope?(obj)); }
    }
}

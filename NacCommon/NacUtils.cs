using Nac.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static NacGlobal;

public static class NacUtils {
    public const string cLoopbackIP = "127.0.0.1";

    public static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey fromKey, TKey toKey) {
        TValue value = dic[fromKey];
        dic.Remove(fromKey);
        dic[toKey] = value;
    }
    public static void Update(object obj, string property, object value) {
        var propInfo = obj.GetType().GetProperty(property);
        var propType = propInfo.PropertyType;
        var targetType = IsNullableType(propType) ? Nullable.GetUnderlyingType(propInfo.PropertyType) : propInfo.PropertyType;
        value = Convert.ChangeType(value, targetType);
        propInfo.SetValue(obj, value, null);
    }
    private static bool IsNullableType(Type type) {
        return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
    }
    public static T Clone<T>(T source) {
        if (!typeof(T).IsSerializable) {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if (Object.ReferenceEquals(source, null)) {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream) {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
    public static int IndexOf<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) {
        int i = 0;
        foreach (var pair in dictionary) {
            if (pair.Key.Equals(key)) {
                return i;
            }
            i++;
        }
        return -1;
    }
    public static int IndexOf<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value) {
        int i = 0;
        foreach (var pair in dictionary) {
            if (pair.Value.Equals(value)) {
                return i;
            }
            i++;
        }
        return -1;
    }
    public static T Cast<T>(object obj) {
        return (T)obj;
    }
    //public static dynamic Cast(object obj, Type t) {
    //    return Convert.ChangeType(obj, t);
    //}
    public static T Protect<T>(Func<T> operation, bool warn = false) where T : class {
        try { return operation(); }
        catch (Exception x) when (G.Log(x, operation)) { if (warn) Warn(); return null; }
    }
    public static void Protect(Action operation, bool warn = false) {
        try { operation(); } catch (Exception x) when (G.Log(x, operation)) { if (warn) Warn(); }
    }
    public static bool Succeeds<T>(Func<object> operation, ref T ret, bool withNull = false, bool warn = false) where T : class {
        try {
            var retVal = operation.Invoke() as T;
            if (retVal != null || withNull) ret = retVal;
            return true;
        }
        catch (Exception x) when (G.Log(x, operation)) { if (warn) Warn(); ret = null; return false; }
    }
    public static bool Succeeds(Action operation, bool warn = false) {
        try { operation.Invoke(); return true; }
        catch (Exception x) when (G.Log(x, operation)) { if (warn) Warn(); return false; }
    }
    public static bool Fails<T>(Func<object> operation, ref T ret) where T : class {
        return !Succeeds(operation, ref ret);
    }
    public static bool Fails(Action operation) {
        return !Succeeds(operation);
    }
    private static void Warn() {
        MessageBox.Show("Operation Failed!", "Warning");
    }

    static Dictionary<Type, List<Type>> dict = new Dictionary<Type, List<Type>>() {
        { typeof(decimal), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char) } },
        { typeof(double), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) } },
        { typeof(float), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) } },
        { typeof(ulong), new List<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(char) } },
        { typeof(long), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char) } },
        { typeof(uint), new List<Type> { typeof(byte), typeof(ushort), typeof(char) } },
        { typeof(int), new List<Type> { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(char) } },
        { typeof(ushort), new List<Type> { typeof(byte), typeof(char) } },
        { typeof(short), new List<Type> { typeof(byte) } }
    };
    public static bool IsCastableTo(this Type from, Type to) {
        if (to.IsAssignableFrom(from)) {
            return true;
        }
        if (dict.ContainsKey(to) && dict[to].Contains(from)) {
            return true;
        }
        bool castable = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .Any(
                            m => m.ReturnType == to &&
                            (m.Name == "op_Implicit" ||
                            m.Name == "op_Explicit")
                        );
        return castable;
    }

    public static void Decouple(Action action, ref Task task) {
        if (task == null || task.IsCompleted) task = Task.Factory.StartNew(action);
    }

    
    public static byte[] Compress(string[] data, string separator = @"!@#") {
        string fullString = string.Join(separator, data);
        var buffer = Encoding.UTF8.GetBytes(fullString);
        using (var msi = new MemoryStream(buffer))
        using (var mso = new MemoryStream()) {
            using (var gs = new GZipStream(mso, CompressionMode.Compress)) msi.CopyTo(gs);
            return mso.ToArray();
        }
    }

    public static string[] Decompress(byte[] data, string separator = @"!@#") {
        using (var msi = new MemoryStream(data))
        using (var mso = new MemoryStream()) {
            using (var gs = new GZipStream(msi, CompressionMode.Decompress)) gs.CopyTo(mso);
            var str = Encoding.UTF8.GetString(mso.ToArray());
            return Regex.Split(str, separator);
        }
    }
    public static void Foreach(this string[] strs, Action<string> action) {
        foreach (var str in strs) action(str);
    }
    public static IEnumerable<T> Foreach<T>(this string[] strs, Func<string, T> function) {
        foreach (var str in strs) yield return function(str);
    }
    public static string FindIP() {
        List<IPAddress> ipList = new List<IPAddress>();
        foreach (var ni in NetworkInterface.GetAllNetworkInterfaces()) {
            foreach (var ua in ni.GetIPProperties().UnicastAddresses) {
                if (ua.Address.AddressFamily == AddressFamily.InterNetwork && ua.Address.ToString().StartsWith("203.15"))
                    ipList.Add(ua.Address);
            }
        }
        if (ipList.Count > 0) return ipList.First().ToString();

        return null;
    }
}

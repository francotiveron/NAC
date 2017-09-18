using Nac.Common;
using System;
using System.Runtime.Serialization;

public interface INacContext : IDisposable {
    //dynamic GetEngine(string path);
    //dynamic Engine { get; }
    //dynamic Field { get; }
    bool Log(params object[] args);
}
public static class NacGlobal {
    public static INacContext G { get; set; }
}
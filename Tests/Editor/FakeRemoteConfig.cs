using System.Collections.Generic;
using Neuston.ConfigurationBlocks;

public class FakeRemoteConfig : IRemoteConfig
{
    Dictionary<string, string> _dict = new();

    public bool TryGetValue(string key, out string value)
    {
        return _dict.TryGetValue(key, out value);
    }

    public void Add(string key, string value)
    {
        _dict[key] = value;
    }
}
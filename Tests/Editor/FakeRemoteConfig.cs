using System.Collections.Generic;

// ReSharper disable once IdentifierTypo
namespace Neuston.ConfigurationBlocks
{
    public class FakeRemoteConfig : IRemoteConfigAdapter
    {
        Dictionary<string, string> dict = new();

        public bool TryGetValue(string key, out string value)
        {
            return dict.TryGetValue(key, out value);
        }

        public void Add(string key, string value)
        {
            dict[key] = value;
        }
    }
}
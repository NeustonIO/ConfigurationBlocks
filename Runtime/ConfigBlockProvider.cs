// ReSharper disable once IdentifierTypo

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Neuston.ConfigurationBlocks
{
    public class ConfigBlockProvider
    {
        IRemoteConfig _remoteConfig;
        IConfigBlockKeyProvider _configBlockKeyProvider;
        Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        public event Action FailedToParseJson;

        public ConfigBlockProvider(IRemoteConfig remoteConfig = null, IConfigBlockKeyProvider configBlockKeyProvider = null)
        {
            _remoteConfig = remoteConfig ?? new RemoteConfigThatAlwaysReturnsFalse();
            _configBlockKeyProvider = configBlockKeyProvider ?? new DefaultConfigBlockKeyProvider();
        }

        public T Get<T>() where T : new()
        {
            if (_cache.TryGetValue(typeof(T), out object cachedValue))
            {
                return (T)cachedValue;
            }

            var configBlock = CreateConfigBlock<T>();
            _cache[typeof(T)] = configBlock;
            return configBlock;
        }

        T CreateConfigBlock<T>() where T : new()
        {
            var key = _configBlockKeyProvider.GetKeyFromConfigBlockType(typeof(T));
            if (_remoteConfig.TryGetValue(key, out string json))
            {
                try
                {
                    return JsonUtility.FromJson<T>(json);
                }
                catch (Exception)
                {
                    FailedToParseJson?.Invoke();
                }
            }

            return new T();
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }

    public interface IRemoteConfig
    {
        bool TryGetValue(string key, out string value);
    }

    public interface IConfigBlockKeyProvider
    {
        string GetKeyFromConfigBlockType(Type configBlockType);
    }

    public class DefaultConfigBlockKeyProvider : IConfigBlockKeyProvider
    {
        public string GetKeyFromConfigBlockType(Type configBlockType)
        {
            return configBlockType.Name;
        }
    }

    public class RemoteConfigThatAlwaysReturnsFalse : IRemoteConfig
    {
        public bool TryGetValue(string key, out string value)
        {
            value = default;
            return false;
        }
    }
}

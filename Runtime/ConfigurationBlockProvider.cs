using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once IdentifierTypo
namespace Neuston.ConfigurationBlocks
{
	public class ConfigurationBlockProvider
	{
		IRemoteConfigAdapter remoteConfig;
		IKeyProvider keyProvider;
		Dictionary<Type, object> cache = new();

		public event Action FailedToParseJson;

		public ConfigurationBlockProvider(IRemoteConfigAdapter remoteConfig = null, IKeyProvider keyProvider = null)
		{
			this.remoteConfig = remoteConfig ?? new RemoteConfigThatAlwaysReturnsFalse();
			this.keyProvider = keyProvider ?? new DefaultKeyProvider();
		}

		public T Get<T>() where T : ConfigurationBlock, new()
		{
			if (cache.TryGetValue(typeof(T), out object cachedValue))
			{
				return (T)cachedValue;
			}

			var configurationBlock = CreateConfigurationBlock<T>();
			cache[typeof(T)] = configurationBlock;
			return configurationBlock;
		}

		T CreateConfigurationBlock<T>() where T : new()
		{
			var key = keyProvider.GetKeyFromConfigBlockType(typeof(T));

			if (remoteConfig.TryGetValue(key, out string json))
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
			cache.Clear();
		}
	}

	public abstract class ConfigurationBlock
	{
	}

	public interface IRemoteConfigAdapter
	{
		bool TryGetValue(string key, out string value);
	}

	public interface IKeyProvider
	{
		string GetKeyFromConfigBlockType(Type configBlockType);
	}

	public class DefaultKeyProvider : IKeyProvider
	{
		public string GetKeyFromConfigBlockType(Type configBlockType)
		{
			return configBlockType.Name;
		}
	}

	public class RemoteConfigThatAlwaysReturnsFalse : IRemoteConfigAdapter
	{
		public bool TryGetValue(string key, out string value)
		{
			value = default;
			return false;
		}
	}
}
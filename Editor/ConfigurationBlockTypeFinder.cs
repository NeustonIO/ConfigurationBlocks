using System;
using System.Collections.Generic;
using UnityEditor;

namespace Neuston.ConfigurationBlocks.Editor
{
	class ConfigurationBlockTypeFinder
	{
		public static IEnumerable<Type> FindConfigurationBlockTypes()
		{
			return TypeCache.GetTypesDerivedFrom<ConfigurationBlock>();
		}
	}
}
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Neuston.ConfigurationBlocks.Editor
{
	public class ConfigurationBlocksEditor : EditorWindow
	{
		[Serializable]
		class Entry
		{
			public string Name { get; set;  }
			public string Json { get; set; }

			public Entry(string name, string json)
			{
				Name = name;
				Json = json;
			}
		}

		bool populated;
		List<Entry> entries = new();
		Vector2 scrollPosition;

		[MenuItem("Tools/Neuston/Configuration Blocks")]
		static void ShowWindow()
		{
			var window = GetWindow<ConfigurationBlocksEditor>();
			window.titleContent = new GUIContent("Configuration Blocks");
			window.Show();
		}

		void OnGUI()
		{
			if (!populated)
			{
				PopulateEntries();
			}

			if (GUILayout.Button("Reset"))
			{
				ClearEntries();
				PopulateEntries();
			}

			Draw();
		}

		void Draw()
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
			DrawEntries();
			EditorGUILayout.EndScrollView();
		}

		void DrawEntries()
		{
			foreach (var entry in entries)
			{
				DrawEntry(entry);
				EditorGUILayout.Separator();
			}
		}

		void DrawEntry(Entry entry)
		{
			EditorGUILayout.LabelField(entry.Name);
			entry.Json = EditorGUILayout.TextArea(entry.Json);
		}

		void PopulateEntries()
		{
			var configurationBlockTypes = ConfigurationBlockTypeFinder.FindConfigurationBlockTypes();

			foreach (var configurationBlockType in configurationBlockTypes)
			{
				var instance = Activator.CreateInstance(configurationBlockType) as ConfigurationBlock;
				var json = JsonUtility.ToJson(instance, true);

				entries.Add(new Entry(configurationBlockType.Name, json));
			}

			entries.Sort(Alphabetically);

			populated = true;
		}

		int Alphabetically(Entry x, Entry y)
		{
			return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
		}

		void ClearEntries()
		{
			entries.Clear();
			populated = false;
		}

		void OnDestroy()
		{
			ClearEntries();
		}
	}
}
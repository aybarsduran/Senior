using System;
using UnityEngine;

namespace IdenticalStudios.SaveSystem
{
	[CreateAssetMenu(menuName = "Identical Studios/Saving/Prefab Database")]
	public sealed class PrefabDatabase : ScriptableObject
	{
		public static event Action Enabled;

		private static PrefabDatabase s_Instance;
		public static PrefabDatabase Default
		{
			get
			{
				if (s_Instance == null)
				{
					var allDatabases = Resources.LoadAll<PrefabDatabase>("");

					if (allDatabases != null && allDatabases.Length > 0)
						s_Instance = allDatabases[0];
				}

				return s_Instance;
			}
		}

		public SaveableObject[] Prefabs
		{
			get => m_Prefabs;
			set => m_Prefabs = value ?? Array.Empty<SaveableObject>();
		}

		[Help("Every prefab with a ''Saveable Object'' script attached (Auto filled).")]

		[SerializeField, ReorderableList(fixedSize: true, HasLabels = false), Disable]
		private SaveableObject[] m_Prefabs = Array.Empty<SaveableObject>();


        private void OnEnable() => Enabled?.Invoke();
	}
}
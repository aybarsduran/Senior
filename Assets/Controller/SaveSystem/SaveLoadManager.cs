using System.Collections.Generic;
using System.IO;
using UnityEngine;
using IdenticalStudios.OdinSerializer;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IdenticalStudios.SaveSystem
{
    using SerializationUtility = OdinSerializer.SerializationUtility;

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
	public static class SaveLoadManager
	{
		public static bool IsSaving => m_IsSaving;

		private const string SAVE_FILE_NAME = "Save";
		private const string SAVE_FILE_EXTENSION = "sav";

		private static bool m_IsSaving;
		private static string m_SaveFilePath;
        private static GameData m_DataToSave;


#if UNITY_EDITOR
		static SaveLoadManager()
		{
			EditorApplication.projectChanged += AssignPrefabs;
			PrefabDatabase.Enabled += AssignPrefabs;
		}

        private static void AssignPrefabs()
		{
			if (PrefabDatabase.Default == null || Application.isPlaying)
				return;

			var allPrefabs = AssetDatabase.FindAssets("t:GameObject");
			var saveablesInProject = new List<SaveableObject>();

			foreach (var prefabGuid in allPrefabs)
			{
				var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(prefabGuid));
				
				if (gameObject.TryGetComponent<SaveableObject>(out var saveable))
				{
					if (saveable.PrefabID != prefabGuid)
					{
						EditorUtility.SetDirty(saveable);
						saveable.PrefabID = prefabGuid;
					}

					saveablesInProject.Add(saveable);
				}
			}

			Resources.UnloadUnusedAssets();
			PrefabDatabase.Default.Prefabs = saveablesInProject.ToArray();
			EditorUtility.SetDirty(PrefabDatabase.Default);
		}
#endif

		public static bool SaveFileExists(int saveId) => File.Exists(GetSaveFilePath(saveId));

		public async static void SaveToFile(GameData game)
		{
            if (IsSaving)
                return;

			var savePath = GetSavePath();

			if (!Directory.Exists(GetSavePath()))
				Directory.CreateDirectory(savePath);

            m_SaveFilePath = GetSaveFilePath(game.SaveId);
            m_DataToSave = game;

			m_IsSaving = true;
			await SaveToFileAsync();

			m_IsSaving = false;
			m_DataToSave = null;
		}

		public static GameData LoadFromSaveFile(int saveId)
		{
			string saveFilePath = GetSaveFilePath(saveId);

			if (!File.Exists(saveFilePath))
				return null;

			byte[] bytes = File.ReadAllBytes(saveFilePath);
			GameData gameData = SerializationUtility.DeserializeValue<GameData>(bytes, DataFormat.Binary);

			return gameData;
		}

		public static List<GameData> LoadAllSaves()
		{
			var saves = new List<GameData>();

			for (int i = 0; i < 10; i++)
			{
				string saveFilePath = GetSaveFilePath(i);

				if (File.Exists(saveFilePath))
				{
					byte[] bytes = File.ReadAllBytes(saveFilePath);
					GameData gameData = SerializationUtility.DeserializeValue<GameData>(bytes, DataFormat.Binary);

					saves.Add(gameData);
				}
			}

			return saves;
		}

		public static void DeleteSaveFile(int saveId)
		{
			if (!File.Exists(GetSaveFilePath(saveId)))
				return;

			File.Delete(GetSaveFilePath(saveId));
		}

		public static SaveableObject GetPrefabWithID(string prefabGuid)
		{
			var prefabs = PrefabDatabase.Default.Prefabs;

			foreach (var prefab in prefabs)
			{
				if (prefab.PrefabID == prefabGuid)
					return prefab;
			}

			return null;
		}

		private async static Task SaveToFileAsync()
        {
            try
            {
                await Task.Run(SaveToFile);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static void SaveToFile()
        {
			Stream stream = File.Open(m_SaveFilePath, FileMode.Create);
            var context = new SerializationContext();
            var writer = new BinaryDataWriter(stream, context);

			SerializationUtility.SerializeValue(m_DataToSave, writer);

            stream.Close();
		}

        private static string GetSavePath() => Application.persistentDataPath + "/Saves";

		private static string GetSaveFilePath(int saveId)
		{
			return GetSavePath() + "/" + SAVE_FILE_NAME + " " + saveId + "." + SAVE_FILE_EXTENSION;
		}
    }
}
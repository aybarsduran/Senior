using System;
using UnityEngine;

namespace IdenticalStudios
{
    [CreateAssetMenu(menuName = "Identical Studios/Misc/Scene Setup Info")]
    public class SceneSetupInfo : ScriptableObject
    {
        #region Internal
        [Serializable]
        public struct SceneSetupPrefab
        {
            public GameObject Prefab;
            public PrefabSetupType SetupType;
        }

        public enum PrefabSetupType
        {
            DontReplaceExisting,
            ReplaceExisting,
            Ignore
        }
        #endregion

        public SceneSetupPrefab[] SceneSetupPrefabs => m_SceneSetupPrefabs;
        public string[] BaseObjectNames => m_EmptyObjects;
        public bool SpawnEmptyObjects => m_SpawnEmptyObjects;
        public bool AddSceneToBuildSettings => m_AddSceneToBuild;

        [SerializeField, ReorderableList(childLabel: "Prefab")]
        private SceneSetupPrefab[] m_SceneSetupPrefabs;

        [SerializeField, ReorderableList(HasLabels = false)]
        private string[] m_EmptyObjects;

        [SerializeField]
        private bool m_SpawnEmptyObjects;

        [SerializeField]
        private bool m_AddSceneToBuild;


        public void ResetToDefault() 
        {
            for (int i = 0; i < m_SceneSetupPrefabs.Length; i++)
                SceneSetupPrefabs[i].SetupType = PrefabSetupType.DontReplaceExisting;

            m_EmptyObjects = null;

            m_SpawnEmptyObjects = true;
            m_AddSceneToBuild = true;
        }
    }
}

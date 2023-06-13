using IdenticalStudios.InputSystem;
using IdenticalStudios.SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using IEnumerator = System.Collections.IEnumerator;

namespace IdenticalStudios
{
    [CreateAssetMenu(menuName = "Identical Studios/Managers/Levels", fileName = "LevelManager")]
    public sealed class LevelManager : Manager<LevelManager>
    {
        public GameData CurrentGame => m_CurrentGame;
        public float LoadingProgress => m_LoadingProgress;
        
        public static bool IsLoading
        {
            get => Instance.m_IsLoading;
            private set
            {
                if (value == Instance.m_IsLoading)
                    return;

                Instance.m_IsLoading = value;

                if (value)
                    GameLoadStart?.Invoke();
                else
                    GameLoaded?.Invoke();
            }
        }
        
        public static bool IsSaving
        {
            get => Instance.m_IsSaving;
            private set
            {
                if (value == Instance.m_IsSaving)
                    return;
                
                Instance.m_IsSaving = value;

                if (value)
                    GameSaveStart?.Invoke();
                else
                    GameSaved?.Invoke();
            }
        }

        public static event UnityAction GameSaveStart;
        public static event UnityAction GameSaved;
        public static event UnityAction GameLoadStart;
        public static event UnityAction GameLoaded;

        [SerializeField, PrefabObjectOnly]
        private GameObject m_LoadScreen;

        [SerializeField]
        private InputContextGroup m_LoadContext;

        [NonSerialized]
        private bool m_IsSaving = false;

        [NonSerialized]
        private bool m_IsLoading = false;

        [NonSerialized]
        private float m_LoadingProgress;

        [NonSerialized]
        private GameData m_CurrentGame;

        private RuntimeObject m_RuntimeObject;
        private readonly List<SaveableObject> m_CurrentSceneSaveables = new();


        #region Initialization
        private class RuntimeObject : MonoBehaviour
        {
            public event UnityAction OnStart;
            private void Start() => OnStart.Invoke();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init() => SetInstance();

        protected override void OnInitialized()
        {
            m_RuntimeObject = CreateRuntimeObject<RuntimeObject>("LevelRuntimeObject");
            m_RuntimeObject.OnStart += OnStart;
            Instantiate(m_LoadScreen, m_RuntimeObject.transform);

            void OnStart()
            {
                m_CurrentGame = new GameData(-1, new SceneData() { Name = SceneManager.GetActiveScene().name });
                GameLoaded?.Invoke();
            }
        }
        #endregion
        
        public static void LoadScene(string sceneName) => Instance.m_RuntimeObject.StartCoroutine(Instance.LoadSceneCoroutine(sceneName));
        public static void LoadGame(int saveId) => Instance.m_RuntimeObject.StartCoroutine(Instance.LoadGameCoroutine(saveId));
        public static bool GameExists(int id) => SaveLoadManager.SaveFileExists(id);
        public static void RemoveGame(int saveId) => SaveLoadManager.DeleteSaveFile(saveId);

        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            IsLoading = true;

            yield return new WaitForSeconds(0.75f);

            // Load the scene
            AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (!sceneLoadOperation.isDone)
            {
                m_LoadingProgress = sceneLoadOperation.progress;
                yield return null;
            }

            yield return null;

            IsLoading = false;
        }

        private IEnumerator LoadGameCoroutine(int saveId)
        {
            GameData game = SaveLoadManager.LoadFromSaveFile(saveId);

            if (game == null || !DoesSceneExist(game.SceneData.Name))
                yield break;

            m_CurrentGame = game;

            IsLoading = true;

            InputManager.PushContext(m_LoadContext);
            m_LoadingProgress = 0f;

            yield return new WaitForSeconds(0.75f);

            // Load the scene
            AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(game.SceneData.Name, LoadSceneMode.Single);

            while (!sceneLoadOperation.isDone)
            {
                m_LoadingProgress = sceneLoadOperation.progress;
                yield return null;
            }

            yield return null;

            // Spawn the player, UI, and others
            LoadSaveables(game.SceneData);

            InputManager.PopContext(m_LoadContext);

            IsLoading = false;
        }

        public static void SaveCurrentGame(int saveId)
        {
            if (IsSaving)
                return;

            Instance.m_RuntimeObject.StartCoroutine(Instance.C_SaveCurrentGame(saveId));
        }

        private IEnumerator C_SaveCurrentGame(int saveId)
        {
            if (m_CurrentGame == null)
                yield break;

            IsSaving = true;

            m_CurrentGame.SaveId = saveId;
            m_CurrentGame.DateTime = DateTime.Now;
            m_CurrentGame.SetScreenshot(ScreenCapture.CaptureScreenshotAsTexture());

            m_CurrentGame.SceneData.Name = SceneManager.GetActiveScene().name;

            Dictionary<string, SaveableObject.Data> saveableObjects = m_CurrentGame.SceneData.Objects;
            saveableObjects.Clear();

            int saveCount = 0;

            for (int i = 0; i < m_CurrentSceneSaveables.Count; i++)
            {
                var sceneSaveable = m_CurrentSceneSaveables[i];
                saveableObjects.Add(sceneSaveable.GetGuid().ToString(), sceneSaveable.GetSaveData());
                saveCount++;

                // Only save 10 objects per frame
                if (saveCount == 10)
                {
                    yield return null;
                    saveCount = 0;
                }
            }

            SaveLoadManager.SaveToFile(m_CurrentGame);
            yield return new WaitUntil(() => !SaveLoadManager.IsSaving);

            IsSaving = false;       
        }

        private void LoadSaveables(SceneData sceneData)
        {
            var saveables = sceneData.Objects;

            for (int i = m_CurrentSceneSaveables.Count - 1; i >= 0; i--)
            {
                if (saveables.TryGetValue(m_CurrentSceneSaveables[i].GetGuid().ToString(), out SaveableObject.Data objData))
                    m_CurrentSceneSaveables[i].LoadData(objData);
                else
                    Destroy(m_CurrentSceneSaveables[i].gameObject);
            }

            foreach (var objData in sceneData.Objects.Values)
            {
                if (!m_CurrentSceneSaveables.Exists((SaveableObject obj) => obj.GetGuid() == new Guid(objData.SceneID)))
                {
                    SaveableObject prefab = SaveLoadManager.GetPrefabWithID(objData.PrefabID);

                    if (prefab != null)
                    {
                        SaveableObject loadedObj = Instantiate(prefab);
                        loadedObj.LoadData(objData);
                    }
                }
            }
        }

        public static void RegisterSaveable(SaveableObject saveable)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;

            if (!Instance.m_CurrentSceneSaveables.Contains(saveable))
                Instance.m_CurrentSceneSaveables.Add(saveable);
            else
                Debug.LogWarning("Saveable is already registered!", saveable);
#else
            Instance.m_CurrentSceneSaveables.Add(saveable);
#endif
        }

        public static void UnregisterSaveable(SaveableObject saveable)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;

            if (!Instance.m_CurrentSceneSaveables.Remove(saveable))
                Debug.LogWarning("Saveable is not registered, no need for un-registering! If you did not get this warning while editing a prefab in playmode something might have gone wrong.", saveable);
#else
            Instance.m_CurrentSceneSaveables.Remove(saveable);
#endif
        }

        /// <summary>
        /// Returns true if the scene 'name' exists and is in your Build settings, false otherwise
        /// </summary>
        private static bool DoesSceneExist(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            for (int i = 0;i < SceneManager.sceneCountInBuildSettings;i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var lastSlash = scenePath.LastIndexOf("/", StringComparison.Ordinal);
                var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".", StringComparison.Ordinal) - lastSlash - 1);

                if (string.Compare(name, sceneName, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
            }

            return false;
        }
    }
}
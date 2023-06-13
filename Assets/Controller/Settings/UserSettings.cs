using System.Linq;
using UnityEngine;

namespace IdenticalStudios
{
    public abstract class UserSettings<T> : UserSettingsBase where T : UserSettings<T>
    {
#if UNITY_EDITOR
        public static T Instance { get; private set; }
#else
        public static T Instance;
#endif

        private const string k_DefaultUserSettingsPath = "Settings/";


        protected static void SetInstance(string path = null)
        {
            if (path == null)
                path = k_DefaultUserSettingsPath;

            var managers = Resources.LoadAll<T>(path);

            Instance = managers.FirstOrDefault();

            if (Instance == null)
                Instance = CreateInstance<T>();

            if (Instance != null)
                Instance.OnInitialized();
        }

        protected virtual void OnInitialized() { }
    }
}
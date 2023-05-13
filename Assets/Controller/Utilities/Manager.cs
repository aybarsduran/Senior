using IdenticalStudios;
using System.Linq;
using UnityEngine;

namespace IdenticalStudios
{
    public abstract class Manager<T> : ManagerBase where T : Manager<T>
    {
        public static bool HasInstance => Instance != null;

        protected static T Instance;

        private const string k_DefaultManagerPath = "Managers/";

        protected static void SetInstance(string path = k_DefaultManagerPath)
        {
            if (path == null)
                path = k_DefaultManagerPath;

            var managers = Resources.LoadAll<T>(path);

            Instance = managers.FirstOrDefault();

            if (Instance == null)
                Instance = CreateInstance<T>();

            Instance.OnInitialized();
        }

        protected static void CreateInstance()
        {
            Instance = CreateInstance<T>();
            Instance.OnInitialized();
        }

        protected U CreateRuntimeObject<U>(string objName = "RuntimeObject") where U : MonoBehaviour
        {
            var managersRoot = GetManagersRoot();
            var runtimeObject = new GameObject(objName).AddComponent<U>();
            runtimeObject.transform.parent = managersRoot.transform;
            runtimeObject.gameObject.name = objName;

            return runtimeObject;
        }

        protected Transform CreateSimpleRuntimeObject(string objName = "RuntimeObject")
        {
            var managersRoot = GetManagersRoot();
            var runtimeObject = new GameObject(objName);
            runtimeObject.transform.parent = managersRoot.transform;
            runtimeObject.name = objName;

            return runtimeObject.transform;
        }

        protected virtual void OnInitialized() { }
    }
}

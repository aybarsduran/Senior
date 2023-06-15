using UnityEngine;

namespace IdenticalStudios
{
    public abstract class ManagerBase : ScriptableObject
    {
        private static GameObject s_ManagersRoot;
        
        
        protected static GameObject GetManagersRoot()
        {
            if (s_ManagersRoot == null)
            {
                s_ManagersRoot = new("Managers")
                {
                    tag = "GameController"
                };

                DontDestroyOnLoad(s_ManagersRoot);
            }

            return s_ManagersRoot;
        }
    }
}
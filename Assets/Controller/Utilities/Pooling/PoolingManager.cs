using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IdenticalStudios.PoolingSystem
{
    public sealed class PoolingManager : Manager<PoolingManager>
    {
        #region Initialization
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init() => CreateInstance();
        
        protected override void OnInitialized()
        {
            m_RuntimeObject = CreateSimpleRuntimeObject("PoolingRuntimeObject");
            SceneManager.sceneUnloaded += ClearPools;

            void ClearPools(Scene scene)
            {
                foreach (var pool in m_Pools.Values)
                    pool.ReleaseInstances();

                m_Pools.Clear();
            }
        }
        #endregion
        
        private readonly Dictionary<int, ObjectPool> m_Pools = new(32);

        private Transform m_RuntimeObject;
        private const int k_DefaultPoolCapacity = 16;


        public static ObjectPool CreatePool(GameObject template, int initialInstances, int capacity, float autoReleaseDelay = Mathf.Infinity)
        {
            if (template == null)
            {
                Debug.LogError("You want to create an object pool for an object that is null!");
                return null;
            }

            var pools = Instance.m_Pools;

            int id = template.GetHashCode();
            if (!pools.TryGetValue(id, out var pool))
            {
                pool = new ObjectPool(template, id, capacity, Instance.m_RuntimeObject.transform, autoReleaseDelay);
                pool.Populate(initialInstances);
                pools.Add(id, pool);
            }

            return pool;
        }

        public static ObjectPool GetPool(int id)
        {
            if (Instance.m_Pools.TryGetValue(id, out var pool))
                return pool;

            return null;
        }

        /// <summary>
        /// This method will use the prefab's instance id as a poolId to create a pool if one doesn't exist (it's the closest thing in ease of use to Object.Instantiate())<br></br>
        /// You can also use CreatePool() to create a custom pool for your prefabs.
        /// </summary>
        /// <returns></returns>
        public static PoolableObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogError("Object cannot be null!");
                return null;
            }

            PoolableObject obj;

            if (Instance.m_Pools.TryGetValue(prefab.GetHashCode(), out var pool))
                obj = pool.GetInstance();
            else
            {
                var newPool = CreatePool(prefab, 0, k_DefaultPoolCapacity);
                Instance.m_Pools.Add(prefab.GetHashCode(), newPool);

                obj = newPool.GetInstance();
            }

            if (obj != null)
            {
                obj.transform.SetPositionAndRotation(position, rotation);
                obj.transform.SetParent(parent, true);
            }

            return obj;
        }

        /// <summary>
        /// This method will use the prefab's instance id as a poolId to create a pool if one doesn't exist (it's the closest thing in ease of use to Object.Instantiate())<br></br>
        /// You can also use CreatePool() to create a custom pool for your prefabs.
        /// </summary>
        /// <returns></returns>
        public static PoolableObject GetObject(int poolId, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (Instance.m_Pools.TryGetValue(poolId, out var pool))
            {
                PoolableObject obj = pool.GetInstance();

                if (obj != null)
                {
                    obj.transform.SetPositionAndRotation(position, rotation);
                    obj.transform.SetParent(parent, true);
                }

                return obj;
            }

            return null;
        }
    }
}


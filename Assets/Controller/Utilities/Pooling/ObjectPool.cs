using System.Collections.Generic;
using UnityEngine;

namespace IdenticalStudios.PoolingSystem
{
    public class ObjectPool
    {
        private readonly PoolableObject m_Template;
        private readonly Queue<PoolableObject> m_Pool;
        private readonly Transform m_Parent;
        private readonly int m_Capacity;


        public ObjectPool(GameObject template, int id, int capacity, Transform parent, float autoReleaseDelay = Mathf.Infinity)
        {
            if (template == null || capacity < 1)
            {
                Debug.LogError("You want to create an object pool for an object that is null!");
                return;
            }

            m_Parent = parent;
            m_Capacity = capacity;
            m_Pool = new(m_Capacity);

            var obj = Object.Instantiate(template);

            if (obj.TryGetComponent(out m_Template))
                m_Template.Init(id);
            else
            {
                m_Template = obj.AddComponent<PoolableObject>();
                m_Template.Init(id);
                m_Template.AutoReleaseDelay = autoReleaseDelay;
            }

            m_Template.gameObject.SetActive(false);
            m_Template.transform.parent = m_Parent;
        }

        public PoolableObject GetInstance()
        {
            PoolableObject instance = GetPooledInstance();
            if (instance == null)
                instance = CreateInstance();
            
            instance.gameObject.SetActive(true);

            return instance;
        }

        public void ReleaseInstance(PoolableObject instance)
        {
            if (instance == null)
            {
                Debug.LogError("The object you want to return is null!");
                return;
            }

            if (m_Pool.Count == m_Capacity)
            {
                Object.Destroy(instance.gameObject);
                return;
            }

            m_Pool.Enqueue(instance);

            instance.transform.SetParent(m_Parent);
            instance.gameObject.SetActive(false);
        }

        public void ReleaseInstances(int keep = 0)
        {
            if ((keep < 0) || (keep > m_Capacity))
                throw new System.ArgumentOutOfRangeException(nameof(keep));

            if (keep != 0)
            {
                for (int i = m_Pool.Count - keep; i > 0; i--)
                {
                    var released = m_Pool.Dequeue();
                    Object.Destroy(released.gameObject);
                }
            }
            else
            {
                while (m_Pool.Count > 0)
                {
                    var released = m_Pool.Dequeue();
                    Object.Destroy(released.gameObject);
                }
            }
        }

        public void Populate(int amount)
        {
            amount = Mathf.Min(amount, m_Capacity - 1);

            while (amount > m_Pool.Count)
                m_Pool.Enqueue(CreateInstance());
        }

        private PoolableObject GetPooledInstance()
        {
            PoolableObject instance = null;
            while ((m_Pool.Count > 0) && (instance == null))
                instance = m_Pool.Dequeue();

            return instance;
        }

        private PoolableObject CreateInstance()
        {
            var instance = Object.Instantiate(m_Template, m_Parent);
            return instance;
        }
    }
}

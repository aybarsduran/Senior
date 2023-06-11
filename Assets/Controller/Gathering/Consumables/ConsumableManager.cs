using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IdenticalStudios
{
    public class ConsumableManager : MonoBehaviour, ISaveableComponent
    {
        #region Internal
        [Serializable]
        public struct RespawnData 
        {
            [NonSerialized]
            public Consumable Consumable;

            public int ConsumableIndex;
            public float RespawnTime;
        }
        #endregion

        [SerializeField]
        private bool m_RespawnConsumables;

        [SerializeField]
        private int m_RespawnTimeMin = 45;

        [SerializeField]
        private int m_RespawnTimeMax = 60;

        private List<RespawnData> m_RespawnDatas;

        private Consumable[] m_Consumables;


        private void Awake()
        {
            m_Consumables = GetComponentsInChildren<Consumable>();
            m_RespawnDatas = new List<RespawnData>(m_Consumables.Length);

            foreach (var consumable in m_Consumables)
                consumable.Consumed += OnItemConsumed;
        }

        private void Update()
        {
            int i = 0;
            float deltaTime = Time.deltaTime;

            while (i < m_RespawnDatas.Count)
            {
                var respawnData = m_RespawnDatas[i];
                respawnData.RespawnTime -= deltaTime;

                if (respawnData.RespawnTime <= 0f)
                {
                    respawnData.Consumable.gameObject.SetActive(true);
                    m_RespawnDatas.Remove(respawnData);
                }
                else
                    i++;
            }
        }

        private void OnItemConsumed(Consumable consumable)
        {
            var newRespawnData = new RespawnData()
            {
                Consumable = consumable,
                RespawnTime = Random.Range(m_RespawnTimeMin, m_RespawnTimeMax),
                ConsumableIndex = GetIndexOfConsumable(consumable)
            };

            m_RespawnDatas.Add(newRespawnData);
        }

        private int GetIndexOfConsumable(Consumable consumable)
        {
            for (int i = 0; i < m_Consumables.Length; i++)
            {
                if (m_Consumables[i] == consumable)
                    return i;
            }

            return -1;
        }

        #region Save & Load
        public void LoadMembers(object[] members)
        {
            m_RespawnDatas = members[0] as List<RespawnData>;

            foreach (var data in m_RespawnDatas)
                m_Consumables[data.ConsumableIndex].gameObject.SetActive(false);
        }

        public object[] SaveMembers()
        {
            return new object[] { m_RespawnDatas };
        }
        #endregion
    }
}

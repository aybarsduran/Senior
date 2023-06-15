using IdenticalStudios.InventorySystem;
using UnityEngine;

namespace IdenticalStudios
{
    public class SoloSurvivalGameMode : GameMode, ISaveableComponent
    {
        [SpaceArea]
        [SerializeField, InLineEditor]
        private InventoryStartupItemsInfo m_StartupItems;

        private bool m_StartupItemsAdded;
        private bool m_FirstSpawn = true;


        protected override void OnPlayerInitialized()
        {
            base.OnPlayerInitialized();

            AddStartupItems();

            if (m_FirstSpawn)
                SetPlayerPosition(GetSpawnData());

            player.HealthManager.Respawn += OnPlayerRespawn;
        }

        protected override SpawnPointData GetSpawnData()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return base.GetSpawnData();
#endif

            SpawnPointData spawnPoint = SpawnPointData.Default;

            // Set the spawn position to the sleeping place.
            if (player.TryGetModule(out ISleepHandler sleepHandler))
                spawnPoint = new SpawnPointData(sleepHandler.LastSleepPosition, sleepHandler.LastSleepRotation);

            return spawnPoint.IsDefault() ? base.GetSpawnData() : spawnPoint;
        }
        
        private void OnPlayerRespawn()
        {
            SetPlayerPosition(GetSpawnData());
            AddStartupItems();
        }

        private void AddStartupItems()
        {
            if (m_StartupItemsAdded)
                return;
            
            m_StartupItems.AddItemsToInventory(player.Inventory);
            m_StartupItemsAdded = true;
        }

        #region Save & Load
        public object[] SaveMembers()
        {
            return new object[] { m_StartupItemsAdded, false };
        }

        public void LoadMembers(object[] members)
        {
            m_StartupItemsAdded = (bool)members[0];
            m_FirstSpawn = (bool)members[1];
        }
        #endregion
    }
}
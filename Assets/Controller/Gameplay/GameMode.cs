using IdenticalStudios.UISystem;
using UnityEngine;

namespace IdenticalStudios
{
    [DefaultExecutionOrder(-999)]
    public abstract class GameMode : MonoBehaviour
    {
        #region Internal
        public enum InitializationModes
        {
            None,
            SpawnFromPrefabs,
            SearchInScene
        }
        #endregion

        [SerializeField]
        protected GameObject m_SceneCamera;

        [SpaceArea]

        [SerializeField]
        protected InitializationModes m_PlayerInitializeMode; 

        [SerializeField, PrefabObjectOnly]
        [EnableIf(nameof(m_PlayerInitializeMode), InitializationModes.SpawnFromPrefabs)]
        protected Player m_PlayerPrefab;

        [SpaceArea]

        [SerializeField]
        protected InitializationModes m_UIIntializeMode;

        [SerializeField, PrefabObjectOnly]
        [EnableIf(nameof(m_UIIntializeMode), InitializationModes.SpawnFromPrefabs)]
        protected PlayerUIManager m_PlayerUIPrefab;

        protected Player player;
        protected PlayerUIManager playerUI;
        
        
        protected virtual void Start()
        {
            LevelManager.GameLoaded += OnGameLoaded;

            if (!LevelManager.IsLoading)
                OnGameLoaded();
        }

        protected virtual void OnDestroy() => LevelManager.GameLoaded -= OnGameLoaded;

        private void OnGameLoaded()
        {
            // Player set up.
            if (TryGetPlayer(out player))
            {
                if (player.IsInitialized) 
                    OnPlayerInitialized();
                else
                    player.AfterInitialized += OnPlayerInitialized;
            }
            else
                Debug.LogError("The Player prefab is null, you need to assign it in the inspector.", gameObject);
            
            // Destroy the scene camera.
            if (m_SceneCamera != null)
            {
                m_SceneCamera.SetActive(false);
                Destroy(m_SceneCamera);
            }
        }

        protected virtual void OnPlayerInitialized()
        {
            player.AfterInitialized -= OnPlayerInitialized;

            // Player UI set up.
            if (TryGetPlayerUI(out playerUI))
                playerUI.AttachToPlayer(player);
            else
                Debug.Log("The Player UI prefab is null, you need to assign it in the inspector.", gameObject);
        }

        private bool TryGetPlayer(out Player player)
        {
            player = null;

            if (m_PlayerInitializeMode == InitializationModes.SearchInScene)
                player = Player.LocalPlayer;
            else if (m_PlayerInitializeMode == InitializationModes.SpawnFromPrefabs)
            {
                if (Player.LocalPlayer != null)
                    player = Player.LocalPlayer;
                else if (m_PlayerPrefab != null)
                    player = Instantiate(m_PlayerPrefab);
            }

            return player != null;
        }

        private bool TryGetPlayerUI(out PlayerUIManager playerUI)
        {
            playerUI = null;

            if (m_UIIntializeMode == InitializationModes.SearchInScene)
                playerUI = PlayerUIManager.LocalPlayerUI;
            else if (m_UIIntializeMode == InitializationModes.SpawnFromPrefabs)
            {
                if (m_PlayerUIPrefab != null)
                    playerUI = Instantiate(m_PlayerUIPrefab);
            }

            return playerUI != null;
        }

        protected void SetPlayerPosition(SpawnPointData spawnData)
        {
            if (player == null)
                return;

            // Set the player's position and rotation.
            if (player.TryGetModule(out ICharacterMotor motor))
                motor.Teleport(spawnData.Position, spawnData.Rotation);
        }

        protected virtual SpawnPointData GetSpawnData()
        {
            SpawnPointData spawnPoint = SpawnPointData.Default;

            if (spawnPoint == SpawnPointData.Default)
            {
                // Search for random spawn point.
                var spawnPoints = SpawnPoint.SpawnPoints;
                if (spawnPoints.Length > 0)
                    spawnPoint = spawnPoints.SelectRandom().GetSpawnPoint();
            }

            if (spawnPoint != SpawnPointData.Default)
                return spawnPoint;
            
            // Snaps the spawn point position to the ground.
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 10f))
                spawnPoint = new SpawnPointData(hitInfo.point + Vector3.up * 0.1f, transform.rotation);
            else
                spawnPoint = new SpawnPointData(transform.position, transform.rotation);

            return spawnPoint;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_SceneCamera == null)
            {
                var camera = GetComponentInChildren<Camera>(true);

                if (camera != null)
                    m_SceneCamera = camera.gameObject;
            }
        }
#endif
    }
}
using UnityEngine;
using UnityEngine.Events;

namespace IdenticalStudios.UISystem
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager LocalPlayerUI => s_LocalPlayerUI;

        public Player Player { get; private set; }
        public bool IsAttached { get; private set; } = false;
    
        // This message will be sent after this UI has been attached to a player.
        public event UnityAction OnAttached;

        private static PlayerUIManager s_LocalPlayerUI;


        public void AttachToPlayer(Player player)
        {
            if (Player == player)
                return;

            Player = player;

            if (Player.IsInitialized)
            {
                IsAttached = true;
                OnAttached?.Invoke();
            }
            else
                Player.Initialized += Attach;

            void Attach()
            {
                Player.Initialized -= Attach;
                IsAttached = true;
                OnAttached?.Invoke();
            }
        }

        private void Awake()
        {
            if (s_LocalPlayerUI != null)
            {
                Debug.LogError("2 or more Player UI Managers are present in the scene, this shouldn't be possible.", s_LocalPlayerUI.gameObject);
                Destroy(this);
            }
            else
                s_LocalPlayerUI = this;
        }

        private void OnDestroy()
        {
            if (s_LocalPlayerUI == this)
                s_LocalPlayerUI = null;
        }
    }
}

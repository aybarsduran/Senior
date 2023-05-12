using UnityEngine.Events;

namespace IdenticalStudios
{
    public class Player : Character
    {
        public static Player LocalPlayer
        {
            get => s_LocalPlayer;
            private set
            {
                if (s_LocalPlayer == value)
                    return;

                s_LocalPlayer = value;
                LocalPlayerChanged?.Invoke(s_LocalPlayer);
            }
        }

        public event UnityAction AfterInitialized;

     
        // Player is the Current Player
        public static event PlayerChangedDelegate LocalPlayerChanged;
        public delegate void PlayerChangedDelegate(Player player);

        private static Player s_LocalPlayer;


        protected override void Awake()
        {
            // If a local player object already exists, destroy this object
            if (LocalPlayer != null)
                Destroy(this);
            else
            {
                LocalPlayer = this;
                base.Awake();
            }
        }

        protected override void Start()
        {
            base.Start();
            AfterInitialized?.Invoke();
        }

        private void OnDestroy()
        {
            if (LocalPlayer == this)
                LocalPlayer = null;
        }
    }
}
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

        /// <summary>
        /// This message will be sent after the first initialized action.
        /// </summary>
        public event UnityAction AfterInitialized;
        
        /// <summary>
        ///  Player: Current Player
        /// </summary>
        public static event PlayerChangedDelegate LocalPlayerChanged;
        public delegate void PlayerChangedDelegate(Player player);

        private static Player s_LocalPlayer;


        protected override void Awake()
        {
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
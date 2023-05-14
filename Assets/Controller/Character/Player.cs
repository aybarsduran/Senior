using IdenticalStudios;
using UnityEngine.Events;

public class Player : Character
{
    // Static property that returns the local player instance
    public static Player LocalPlayer
    {
        get => s_LocalPlayer;
        private set
        {
            // If the local player instance is already set to the given value, return
            if (s_LocalPlayer == value)
                return;
           
            s_LocalPlayer = value;
            // Invoke the LocalPlayerChanged event with the new local player instance
            LocalPlayerChanged?.Invoke(s_LocalPlayer);
        }
    }

    // Event that is invoked after the player object is initialized
    public event UnityAction AfterInitialized;

    // Event that is invoked when the local player changes
    public static event PlayerChangedDelegate LocalPlayerChanged;
    public delegate void PlayerChangedDelegate(Player player);

    // Static field that holds the local player instance
    private static Player s_LocalPlayer;

   
    protected override void Awake()
    {
        // If a local player object already exists, destroy this object
        if (LocalPlayer != null)
            Destroy(this);
        // Otherwise, set this object as the local player and call the base Awake method
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
        // If this object is the local player, set the local player instance to null
        if (LocalPlayer == this)
            LocalPlayer = null;
    }
}

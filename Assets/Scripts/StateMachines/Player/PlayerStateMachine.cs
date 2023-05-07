using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public InputReader InputReader { get; private set; }
    //bu bir property, o yüzden editorde gozukmeyecek.Editor field larý gosterir.
    //SerializeField yaparsak bu sadece fieldlar icin uygulanýyor. O yüzden basýna field: ekledik.
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public float FreeLookMovementSpeed { get; private set; }

    [field: SerializeField] public float RotationDamping { get; private set; }
    [field: SerializeField] public Animator Animator{ get; private set; }
    public Transform MainCameraTransform { get; private set; }


    private void Start()
    {
        MainCameraTransform = Camera.main.transform;
        SwitchState(new PlayerFreeLookState(this));
    }

}

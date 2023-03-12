using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public InputReader InputReader { get; private set; } 
    //bu bir property, o yüzden editorde gozukmeyecek.Editor field larý gosterir.
    //SerializeField yaparsak bu sadece fieldlar icin uygulanýyor. O yüzden basýna field: ekledik.


    private void Start()
    {
        SwitchState(new PlayerTestState(this));
    }

}

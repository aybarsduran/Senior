using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public InputReader InputReader { get; private set; } 
    //bu bir property, o y�zden editorde gozukmeyecek.Editor field lar� gosterir.
    //SerializeField yaparsak bu sadece fieldlar icin uygulan�yor. O y�zden bas�na field: ekledik.


    private void Start()
    {
        SwitchState(new PlayerTestState(this));
    }

}

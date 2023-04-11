using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField]public InputReader InputReader { get; private set; }
    
    private void Start()
    {
        SwitchState(new PlayerTestState(this));
    }
     
}

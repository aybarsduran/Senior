using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour //because this time state machine onto our player/enemies
{
    private State currentState;

    public void SwitchState(State newState) // consistency se bu niye public?
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
     // Update is called once per frame
    private void Update() //just for consistency it is prvate
    {
        currentState?.Tick(Time.deltaTime);
    }

}

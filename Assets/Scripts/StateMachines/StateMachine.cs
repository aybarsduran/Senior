using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private State currentState;
    public void SwitchState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    private void Update()
    {
        currentState?.Tick(Time.deltaTime); //? isareti null degilse calis demek.
    }

   
}

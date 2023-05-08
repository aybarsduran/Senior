using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine; //sadece bu classý inherit edenler ulaþabilsin diye protected yaptýk.

    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;

    }

    protected void Move(Vector3 motion, float deltaTime)
    {
        stateMachine.Controller.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
    }
    protected void FaceTarget()
    {
        if(stateMachine.Targeter.CurrentTarget == null) { return; }
        
        Vector3 lookPosition = stateMachine.Targeter.CurrentTarget.transform.position - stateMachine.transform.position;
        lookPosition.y = 0f;   //height difference we dont care
        
        stateMachine.transform.rotation = Quaternion.LookRotation(lookPosition);
    }
}

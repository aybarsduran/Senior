using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestState : PlayerBaseState
{
    
    public PlayerTestState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        
    }
    public override void Tick(float deltaTime)
    {
        Vector3 movement = new Vector3();

        movement.x = stateMachine.InputReader.MovementValue.x;
        movement.y = 0;
        movement.z = stateMachine.InputReader.MovementValue.y;
       
        stateMachine.Controller.Move(movement * stateMachine.FreeLookMovementSpeed * deltaTime);
        
        if(stateMachine.InputReader.MovementValue == Vector2.zero) 
        {
            stateMachine.Animator.SetFloat("FreeLookSpeed", 0,0.1f,deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat("FreeLookSpeed", 1, 0.1f, deltaTime);
        stateMachine.transform.rotation = Quaternion.LookRotation(movement);

    }

    public override void Exit()
    {
      


    }
    
    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerTestState(stateMachine));
    }

    
}

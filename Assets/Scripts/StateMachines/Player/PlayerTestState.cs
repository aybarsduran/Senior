 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestState : PlayerBaseState
{
    private float timer;

    public PlayerTestState(PlayerStateMachine stateMachine) : base(stateMachine){}

    public override void Enter()
    {

    }
    public override void Tick(float deltaTime)
    {
        Vector3 movement = new Vector3();
        movement.x = stateMachine.InputReader.MovementValue.x;
        movement.y = 0; //we don't want to any player fly :)
        movement.z = stateMachine.InputReader.MovementValue.y;

        //stateMachine.transform.Translate(movement * deltaTime );
        stateMachine.CharacterController.Move(movement * stateMachine.FreeLookMovementSpeed * deltaTime);
        //for see n the console debug
        //Debug.Log(stateMachine.InputReader.MovementValue);
    }
    public override void Exit()
    {
    }

   

}

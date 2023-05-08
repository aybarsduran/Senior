using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    private readonly int TargetingBlendTreeHash = Animator.StringToHash("TargetingBlendTree");
    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.TargetEvent += OnCancel;

        stateMachine.Animator.Play(TargetingBlendTreeHash);
    }

   
    public override void Tick(float deltaTime)
    {
        if(stateMachine.Targeter.CurrentTarget == null)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }

        Vector3 movement = CalculateMovement();

        Move(movement * stateMachine.TargetingMovementSpeed, deltaTime);

        FaceTarget();


    }

    public override void Exit()
    {
        stateMachine.InputReader.TargetEvent -= OnCancel;
    }

    private void OnCancel()
    {
        stateMachine.Targeter.Cancel(); 
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }

    private Vector3 CalculateMovement()
    {
        Vector3 movement = new Vector3();
        movement += stateMachine.transform.right * stateMachine.InputReader.MovementValue.x;
        movement += stateMachine.transform.forward * stateMachine.InputReader.MovementValue.y;
        return movement;
    }
}

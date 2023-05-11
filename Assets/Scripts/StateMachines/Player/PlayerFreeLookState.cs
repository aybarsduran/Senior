 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    //private float timer;
    private readonly int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");

    private readonly int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed"); //readonly it con not changed again, int even faster

    private const float AnimatorDampTime = 0.1f;

    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine){}

    public override void Enter()
    {
        stateMachine.InputReader.TargetEvent += OnTarget;

        stateMachine.Animator.Play(FreeLookBlendTreeHash);
    }
    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputReader.IsAttacking)
        {
            stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0)); 
            return;
        }

        Vector3 movement = CalculateMovement();
        //movement.x = stateMachine.InputReader.MovementValue.x;
        //movement.y = 0; //we don't want to any player fly :)
        //movement.z = stateMachine.InputReader.MovementValue.y;

        Move(movement * stateMachine.FreeLookMovementSpeed, deltaTime);

        //  stateMachine.transform.Translate(movement * deltaTime );
        //stateMachine.CharacterController.Move(movement * stateMachine.FreeLookMovementSpeed * deltaTime);
        //for see n the console debug
        //Debug.Log(stateMachine.InputReader.MovementValue);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, 0.1f, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 1, AnimatorDampTime, deltaTime);
        
        FaceMovementDirection(movement, deltaTime); //yüzü bize doneildi
    }
        
    public override void Exit()
    {
        stateMachine.InputReader.TargetEvent -= OnTarget;

    }

    private void OnTarget()
    {
        if(!stateMachine.Targeter.SelectTarget()) { return; }

        stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
    }


    private Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.forward;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * stateMachine.InputReader.MovementValue.y +
            right * stateMachine.InputReader.MovementValue.x;
    }

    private void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationDamping);
    }
}

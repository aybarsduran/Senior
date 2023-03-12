using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestState : PlayerBaseState
{
    private float timer;
    public PlayerTestState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        stateMachine.InputReader.JumpEvent += OnJump; // OnJump methoduna abone olduk.
    }
    public override void Tick(float deltaTime)
    {
        Debug.Log("Tick");
        Debug.Log(timer);
        timer += deltaTime;
       
    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= OnJump; // OnJump methoduna abone olmaktan cýktýk.


    }
    
    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerTestState(stateMachine));
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing_SlimeMoveState : MoveState
{
    private DemonKing demonKing;

    public DemonKing_SlimeMoveState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, DemonKing demonKing) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.demonKing = demonKing;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isDetectingLedge || isDetectingWall)
        {
            demonKing.slimeIdleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(demonKing.slimeIdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

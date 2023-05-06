using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing_MoveState : MoveState
{
    private DemonKing demonKing;

    public DemonKing_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, DemonKing demonKing) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(demonKing.playerDetectedState);
        }
        else if (!isDetectingLedge || isDetectingWall)
        {
            demonKing.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(demonKing.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

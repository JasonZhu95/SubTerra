using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing_PlayerDetectedState : PlayerDetectedState
{
    private DemonKing demonKing;

    public DemonKing_PlayerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, DemonKing demonKing) : base(entity, stateMachine, animBoolName, stateData)
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

        if (performCloseRangeAction)
        {
            stateMachine.ChangeState(demonKing.meleeAttackState);
        }
        else if (performLongRangeAction)
        {
            stateMachine.ChangeState(demonKing.chargeState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(demonKing.lookForPlayerState);
        }
        else if (!isDetectingLedge)
        {
            Movement?.Flip();
            stateMachine.ChangeState(demonKing.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

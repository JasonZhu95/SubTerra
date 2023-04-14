using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing_ChargeState : ChargeState
{
    private DemonKing demonKing;

    public DemonKing_ChargeState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, DemonKing demonKing) : base(entity, stateMachine, animBoolName, stateData)
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
        else if (!isDectectingLedge || isDetectingWall)
        {
            stateMachine.ChangeState(demonKing.lookForPlayerState);
        }
        else if (isChargeTimeOver)
        {
            if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(demonKing.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(demonKing.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

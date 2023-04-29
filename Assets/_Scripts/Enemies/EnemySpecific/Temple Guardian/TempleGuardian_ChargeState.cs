using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_ChargeState : ChargeState
{
    private TempleGuardian templeGuardian;

    public TempleGuardian_ChargeState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.templeGuardian = templeGuardian;
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
            stateMachine.ChangeState(templeGuardian.meleeAttackState);
        }
        else if (!isDectectingLedge || isDetectingWall)
        {
            stateMachine.ChangeState(templeGuardian.lookForPlayerState);
        }
        else if (isChargeTimeOver)
        {
            if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(templeGuardian.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(templeGuardian.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

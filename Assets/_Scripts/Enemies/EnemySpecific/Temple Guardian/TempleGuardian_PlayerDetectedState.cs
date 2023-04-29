using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_PlayerDetectedState : PlayerDetectedState
{
    private TempleGuardian templeGuardian;

    public TempleGuardian_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName, stateData)
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
        else if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(templeGuardian.chargeState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(templeGuardian.lookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

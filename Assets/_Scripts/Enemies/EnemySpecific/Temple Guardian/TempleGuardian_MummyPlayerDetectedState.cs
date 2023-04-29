using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_MummyPlayerDetectedState : PlayerDetectedState
{
    private TempleGuardian templeGuardian;

    public TempleGuardian_MummyPlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName, stateData)
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
            stateMachine.ChangeState(templeGuardian.mummyMeleeAttackState);
        }
        else if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(templeGuardian.mummyChargeState);
        }
        else if (!isPlayerInMaxAgroRange)
        {
            stateMachine.ChangeState(templeGuardian.mummyLookForPlayerState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

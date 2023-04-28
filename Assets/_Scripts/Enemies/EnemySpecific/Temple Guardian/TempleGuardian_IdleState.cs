using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_IdleState : IdleState
{
    private TempleGuardian templeGuardian;

    public TempleGuardian_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName, stateData)
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

        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(templeGuardian.playerDetectedState);
        }
        else if (isIdleTimeOver)
        {
            stateMachine.ChangeState(templeGuardian.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

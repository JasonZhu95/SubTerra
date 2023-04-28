using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_MoveState : MoveState
{
    private TempleGuardian templeGuardian;

    public TempleGuardian_MoveState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName, stateData)
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
        else if (!isDetectingLedge || isDetectingWall)
        {
            templeGuardian.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(templeGuardian.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

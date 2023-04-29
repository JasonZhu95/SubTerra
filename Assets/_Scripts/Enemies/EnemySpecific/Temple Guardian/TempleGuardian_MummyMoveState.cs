using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_MummyMoveState : MoveState
{
    private TempleGuardian templeGuardian;

    public TempleGuardian_MummyMoveState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName, stateData)
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
            stateMachine.ChangeState(templeGuardian.mummyPlayerDetectedState);
        }
        else if (!isDetectingLedge || isDetectingWall)
        {
            templeGuardian.mummyIdleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(templeGuardian.mummyIdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

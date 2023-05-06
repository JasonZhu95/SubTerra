using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_MoveState : MoveState
{
    private Ranger ranger;

    public Ranger_MoveState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, Ranger ranger) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.ranger = ranger;
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
            stateMachine.ChangeState(ranger.playerDetectedState);
        }
        else if (!isDetectingLedge || isDetectingWall || !isPlayerInMaxAgroRange)
        {
            ranger.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(ranger.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

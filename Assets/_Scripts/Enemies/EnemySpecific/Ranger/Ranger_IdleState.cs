using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_IdleState : IdleState
{
    private Ranger ranger;

    public Ranger_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Ranger ranger) : base(entity, stateMachine, animBoolName, stateData)
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
        else if (isIdleTimeOver)
        {
            stateMachine.ChangeState(ranger.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

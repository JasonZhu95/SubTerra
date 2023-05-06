using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing_IdleState : IdleState
{
    private DemonKing demonKing;

    private bool isPlayerInBossRange;

    public DemonKing_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, DemonKing demonKing) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(demonKing.playerDetectedState);
        }
        else if (isIdleTimeOver)
        {
            stateMachine.ChangeState(demonKing.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing_SlimeIdleState : IdleState
{
    private DemonKing demonKing;

    public DemonKing_SlimeIdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, DemonKing demonKing) : base(etity, stateMachine, animBoolName, stateData)
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

        if (isIdleTimeOver)
        {
            stateMachine.ChangeState(demonKing.slimeMoveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

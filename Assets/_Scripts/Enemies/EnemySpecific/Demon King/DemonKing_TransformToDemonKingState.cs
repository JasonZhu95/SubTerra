using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing_TransformToDemonKingState : TransformToDemonKingState
{
    private DemonKing demonKing;

    public DemonKing_TransformToDemonKingState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_TransformToDemonKingState stateData, DemonKing demonKing) : base(etity, stateMachine, animBoolName, stateData)
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TransformationToDemonKingFinished()
    {
        base.TransformationToDemonKingFinished();

        stateMachine.ChangeState(demonKing.idleState);
    }
}

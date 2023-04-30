using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformToDemonKingState : State
{
    protected D_TransformToDemonKingState stateData;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public TransformToDemonKingState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_TransformToDemonKingState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        entity.atsm.transformToDemonKingState = this;

        Movement?.SetVelocityX(0f);
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

    public virtual void TransformationToDemonKingFinished()
    {

    }
}

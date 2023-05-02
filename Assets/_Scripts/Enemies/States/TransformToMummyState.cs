using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformToMummyState : State
{
    protected D_TransformToMummyState stateData;
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public TransformToMummyState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_TransformToMummyState stateData) : base(etity, stateMachine, animBoolName)
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

        entity.atsm.transformToMummyState = this;

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

        Movement?.SetVelocityX(0f);
    }

    public virtual void TransformationToMummyFinished()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformToHumanState : State
{
    protected D_TransformToHumanState stateData;
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public TransformToHumanState(Entity etity, FiniteStateMachine stateMachine, string animBoolName) : base(etity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        entity.atsm.transformToHumanState = this;

        Movement?.SetVelocityX(0f);
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

    public virtual void TransformationToHumanFinished()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using Project.CoreComponents;
using UnityEngine;

public class AttackState : State
{
    protected Transform attackPosition;

    protected bool isAnimationFinished;
    protected bool isPlayerInMinAgroRange;
    protected bool isPlayerInMidAgroRange;
    protected bool isPlayerInMaxAgroRange;

    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);
    private Movement movement;

    private ParryComponent parryComponent;

    private ParryComponent ParryComponent =>
        parryComponent ? parryComponent : core.GetCoreComponent(ref parryComponent);

    public AttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition) : base(etity, stateMachine, animBoolName)
    {
        this.attackPosition = attackPosition;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAggroRange();
        isPlayerInMidAgroRange = entity.CheckPlayerInMidRangeAction();
        isPlayerInMaxAgroRange = entity.CheckPlayerInMaxAggroRange();
    }

    public override void Enter()
    {
        base.Enter();

        entity.atsm.attackState = this;
        isAnimationFinished = false;
        Movement?.SetVelocityX(0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public virtual void TriggerAttack()
    {

    }

    public virtual void FinishAttack()
    {
        isAnimationFinished = true;
    }

    public void SetParryWindowActive(bool value)
    {
        ParryComponent.SetParryColliderActive(value);
    }
}

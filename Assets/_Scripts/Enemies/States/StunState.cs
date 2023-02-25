using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : State
{
    protected bool isStunTimeOver;
    protected bool isGrounded;
    protected bool isMovementStopped;
    protected bool performCloseRangeAction;
    protected bool isPlayerInMinAgroRange;

    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);
    private Movement movement;

    private CollisionSenses CollisionSenses => collisionSenses ? collisionSenses : core.GetCoreComponent(ref collisionSenses);
    private CollisionSenses collisionSenses;

    protected D_StunState stateData;

    public StunState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = CollisionSenses.Ground;
        performCloseRangeAction = entity.CheckPlayerInCloseRangeAction();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAggroRange();
    }

    public override void Enter()
    {
        base.Enter();

        isStunTimeOver = false;
        isMovementStopped = false;
    }

    public override void Exit()
    {
        base.Exit();
        entity.ResetStunResistance();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(Time.time >= startTime + stateData.stunTime)
        {
            isStunTimeOver = true;
        }

        if(isGrounded && Time.time >= startTime + stateData.stunKnockbackTime && !isMovementStopped)
        {
            isMovementStopped = true;
            Movement?.SetVelocityX(0f);
        }
    }
}

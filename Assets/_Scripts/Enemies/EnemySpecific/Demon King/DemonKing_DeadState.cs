using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing_DeadState : DeadState
{
    private DemonKing demonKing;

    public DemonKing_DeadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, DemonKing demonKing) : base(entity, stateMachine, animBoolName, stateData)
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

    public override void TriggerDeathParticles()
    {
        base.TriggerDeathParticles();
    }

    public override void DieAnimationFinished()
    {
        base.DieAnimationFinished();

        core.transform.parent.gameObject.SetActive(false);
    }
}

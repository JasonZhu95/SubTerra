using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_DeadState : DeadState
{
    private Ranger ranger;
    public Ranger_DeadState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, Ranger ranger) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.ranger = ranger;
    }

    public override void DieAnimationFinished()
    {
        base.DieAnimationFinished();

        core.transform.parent.gameObject.SetActive(false);
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
}

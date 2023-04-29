using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_DeadState : DeadState
{
    private TempleGuardian templeGuardian;

    public TempleGuardian_DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.templeGuardian = templeGuardian;
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

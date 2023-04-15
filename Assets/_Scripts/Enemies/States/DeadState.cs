using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State
{
    protected D_DeadState stateData;

    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    public DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();

        entity.atsm.deadState = this;

        // GameObject.Instantiate(stateData.deathBloodParticle, entity.transform.position, stateData.deathBloodParticle.transform.rotation);
        // GameObject.Instantiate(stateData.deathChunkParticle, entity.transform.position, stateData.deathChunkParticle.transform.rotation);

        // entity.gameObject.SetActive(false);

        Movement?.SetVelocityX(0f);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        Movement?.SetVelocityX(0f);
    }

    public virtual void TriggerDeathParticles()
    {

    }

    public virtual void DieAnimationFinished()
    {

    }
}

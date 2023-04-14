﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State
{
    protected D_DeadState stateData;

    public DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        base.Enter();

        GameObject.Instantiate(stateData.deathBloodParticle, entity.transform.position, stateData.deathBloodParticle.transform.rotation);
        GameObject.Instantiate(stateData.deathChunkParticle, entity.transform.position, stateData.deathChunkParticle.transform.rotation);

        entity.gameObject.SetActive(false);
    }

    public virtual void TriggerDeathParticles()
    {

    }

    public virtual void DieAnimationFinished()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_BeamAttackState : BeamAttackState
{
    Ranger ranger;

    public Ranger_BeamAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_BeamAttackState stateData, Ranger ranger) : base(entity, stateMachine, animBoolName, attackPosition, stateData)
    {
        this.ranger = ranger;
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

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (isPlayerInMaxAgroRange)
            {
                stateMachine.ChangeState(ranger.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(ranger.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }
}

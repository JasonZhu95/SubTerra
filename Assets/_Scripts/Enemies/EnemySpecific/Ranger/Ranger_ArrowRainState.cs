using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_ArrowRainState : ArrowRain
{
    private Ranger ranger;

    public Ranger_ArrowRainState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_ArrowRainState stateData, Ranger ranger) : base(etity, stateMachine, animBoolName, attackPosition, stateData)
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

        if (isAnimationFinished)
        {
            if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(ranger.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(ranger.lookForPlayerState);
            }
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_RangedAttackState : RangedAttackState
{
    private Ranger ranger;

    public Ranger_RangedAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangedAttackState stateData, Ranger ranger) : base(entity, stateMachine, animBoolName, attackPosition, stateData)
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

    public override void LogicUpdate()
    {
        base.LogicUpdate();

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

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

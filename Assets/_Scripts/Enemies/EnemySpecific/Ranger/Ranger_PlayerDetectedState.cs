using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_PlayerDetectedState : PlayerDetectedState
{
    private Ranger ranger;

    public Ranger_PlayerDetectedState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, Ranger ranger) : base(entity, stateMachine, animBoolName, stateData)
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

        if (performCloseRangeAction)
        {
            if (Time.time >= ranger.dodgeState.startTime + ranger.dodgeStateData.dodgeCooldown)
            {
                stateMachine.ChangeState(ranger.dodgeState);
            }
            else
            {
                stateMachine.ChangeState(ranger.meleeAttackState);
            }
        }
        else if (isPlayerInMidAgroRange)
        {
            stateMachine.ChangeState(ranger.beamAttackState);
        }
        //else if (performLongRangeAction)
        //{
        //    stateMachine.ChangeState(ranger.rangedAttackState);
        //}
        //else if (!isPlayerInMaxAgroRange)
        //{
        //    stateMachine.ChangeState(ranger.lookForPlayerState);
        //}
        else
        {
            stateMachine.ChangeState(ranger.rangedAttackState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

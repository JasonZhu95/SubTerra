using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_LookForPlayerState : LookForPlayerState
{
    private Ranger ranger;

    public Ranger_LookForPlayerState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_LookForPlayer stateData, Ranger ranger) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isPlayerInMinAgroRange)
        {
            stateMachine.ChangeState(ranger.playerDetectedState);
        }
        else if (isAllTurnsTimeDone)
        {
            //stateMachine.ChangeState(ranger.moveState);
            stateMachine.ChangeState(ranger.arrowRainState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

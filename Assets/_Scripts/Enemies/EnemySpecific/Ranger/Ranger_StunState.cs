using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranger_StunState : StunState
{
    private Ranger ranger;

    public Ranger_StunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData, Ranger ranger) : base(entity, stateMachine, animBoolName, stateData)
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

        if (isStunTimeOver)
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

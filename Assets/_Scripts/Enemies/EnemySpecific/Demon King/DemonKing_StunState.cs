using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKing_StunState : StunState
{
    private DemonKing demonKing;

    public DemonKing_StunState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData, DemonKing demonKing) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.demonKing = demonKing;
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
                stateMachine.ChangeState(demonKing.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(demonKing.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

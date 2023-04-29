using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_TransformToHumanState : TransformToHumanState
{
    private TempleGuardian templeGuardian;

    public TempleGuardian_TransformToHumanState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName)
    {
        this.templeGuardian = templeGuardian;
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

    public override void TransformationToHumanFinished()
    {
        base.TransformationToHumanFinished();

        stateMachine.ChangeState(templeGuardian.idleState);
    }
}

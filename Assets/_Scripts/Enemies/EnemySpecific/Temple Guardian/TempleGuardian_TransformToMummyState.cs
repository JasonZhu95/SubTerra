using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_TransformToMummyState : TransformToMummyState
{
    private TempleGuardian templeGuardian;

    public TempleGuardian_TransformToMummyState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_TransformToMummyState stateData, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName, stateData)
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

    public override void TransformationAnimationFinished()
    {
        base.TransformationAnimationFinished();

        stateMachine.ChangeState(templeGuardian.mummyMoveState);
    }
}

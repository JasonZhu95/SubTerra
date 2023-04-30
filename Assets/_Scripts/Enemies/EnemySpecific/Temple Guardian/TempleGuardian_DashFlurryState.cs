using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempleGuardian_DashFlurryState : DashFlurryState
{
    private TempleGuardian templeGuardian;

    private Vector3 playerPosition;

    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);
    private Movement movement;

    public TempleGuardian_DashFlurryState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_DashFlurryState stateData, TempleGuardian templeGuardian) : base(etity, stateMachine, animBoolName, attackPosition, stateData)
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

    public override void FinishAttack()
    {
        base.FinishAttack();

        stateMachine.ChangeState(templeGuardian.moveState);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TeleportToPlayer()
    {
        base.TeleportToPlayer();

        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Movement?.SetLocation(playerPosition + new Vector3(0f, -1f, 0f));
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }
}

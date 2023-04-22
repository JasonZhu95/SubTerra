using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Projectiles;

public class RangedAttackState : AttackState
{
    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);
    private Movement movement;

    protected D_RangedAttackState stateData;

    protected GameObject projectile;
    protected Projectile projectileScript;

    public RangedAttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangedAttackState stateData) : base(etity, stateMachine, animBoolName, attackPosition)
    {
        this.stateData = stateData;
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        float arrowAngle;

        if (Movement?.FacingDirection == 1)
        {
            arrowAngle = 0f;
        } else
        {
            arrowAngle = 180f;
        }

        projectile = GameObject.Instantiate(stateData.projectile, attackPosition.position, Quaternion.Euler(0f, 0f, arrowAngle));
        projectileScript = projectile.GetComponent<Projectile>();

        projectileScript.CreateProjectile(stateData.projectileData);
        projectileScript.Init(entity.gameObject);
    }
}

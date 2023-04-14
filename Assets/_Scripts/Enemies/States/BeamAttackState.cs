using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAttackState : AttackState
{
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    protected D_BeamAttackState stateData;

    protected AttackDetails attackDetails;

    public BeamAttackState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_BeamAttackState stateData) : base(entity, stateMachine, animBoolName, attackPosition)
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

        attackDetails.damageAmount = stateData.beamDamage;
        attackDetails.position = entity.transform.position;
        attackDetails.knockbackAngle = stateData.knockbackAngle;
        attackDetails.knockbackStrength = stateData.knockbackStrength;
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

        Collider2D[] detectedObjects = Physics2D.OverlapBoxAll(attackPosition.position, stateData.beamDimensions, 0f, stateData.whatIsPlayer);

        foreach (Collider2D collider in detectedObjects)
        {
            //IDamageable damageable = collider.GetComponent<IDamageable>();
            //IKnockbackable knockbackable = collider.GetComponent<IKnockbackable>();

            //if (damageable != null)
            //{
            //    damageable.Damage(attackDetails);
            //}

            //if (knockbackable != null)
            //{
            //    knockbackable.Knockback(attackDetails.knockbackAngle, attackDetails.knockbackStrength, Movement.FacingDirection);
            //}
        }

    }
}

using Project.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAttackState : AttackState
{
    private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
    private Movement movement;

    protected D_BeamAttackState stateData;

    private DamageData damageData;

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
            IDamageable damageable = collider.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageData.SetData(core.Parent, stateData.beamDamage);
                damageable.Damage(damageData);
            }

            IKnockbackable knockbackable = collider.GetComponent<IKnockbackable>();

            if (knockbackable != null)
            {
                var data = new KnockbackData(stateData.knockbackAngle, stateData.knockbackStrength,
                    Movement.FacingDirection, core.transform.parent.gameObject);
                knockbackable.Knockback(data);
            }

            IPoiseDamageable poiseDamageable = collider.GetComponent<IPoiseDamageable>();

            if (poiseDamageable != null)
            {
                var data = new PoiseDamageData();
                data.Source = core.transform.parent.gameObject;
                data.PoiseDamageAmount = stateData.poiseDamage;
                poiseDamageable.PoiseDamage(data);
            }
        }

    }
}

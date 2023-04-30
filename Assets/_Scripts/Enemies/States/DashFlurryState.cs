using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;

public class DashFlurryState : AttackState
{
    protected D_DashFlurryState stateData;

    private BoxCollider2D boxCollider;

    private DamageData damageData;

    private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);
    private Movement movement;

    public DashFlurryState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_DashFlurryState stateData) : base(etity, stateMachine, animBoolName, attackPosition)
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

        entity.atsm.dashFlurryState = this;

        Movement.RB.constraints = RigidbodyConstraints2D.FreezeAll;

        boxCollider = entity.GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;

        Movement?.SetVelocityZero();
    }

    public override void Exit()
    {
        base.Exit();

        Movement.RB.constraints = RigidbodyConstraints2D.FreezeRotation;

        boxCollider.enabled = true;
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

        Movement?.SetVelocityZero();
    } 

    public virtual void TeleportToPlayer()
    {
        
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackPosition.position, stateData.attackRadius, stateData.whatIsPlayer);

        foreach (Collider2D collider in detectedObjects)
        {
            if (collider.transform.root.gameObject.layer != LayerMask.NameToLayer("PlayerInvincible"))
            {
                IDamageable damageable = collider.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    damageData.SetData(core.Parent, stateData.attackDamage);
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
}

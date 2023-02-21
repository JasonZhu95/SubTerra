using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackReceiver : CoreComponent, IKnockbackable
{
    [SerializeField] private float maxKnockbackTime = 0.2f;

    private CoreComp<Movement> movement;
    private CoreComp<CollisionSenses> collisionSenses;

    private bool isKnockbackActive;
    private float knockbackStartTime;

    public override void LogicUpdate()
    {
        CheckKnockback();
    }

    public void Knockback(Vector2 angle, float strength, int direction)
    {
        movement.Comp?.SetVelocity(strength, angle, direction);
        movement.Comp.CanSetVelocity = false;
        isKnockbackActive = true;
        knockbackStartTime = Time.time;
    }

    private void CheckKnockback()
    {
        if(isKnockbackActive && ((movement.Comp.CurrentVelocity.y <= 0.01f && collisionSenses.Comp.Ground) ||
            Time.time >= knockbackStartTime + maxKnockbackTime))
        {
            isKnockbackActive = false;
            movement.Comp.CanSetVelocity = true;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        movement = new CoreComp<Movement>(core);
        collisionSenses = new CoreComp<CollisionSenses>(core);
    }
}

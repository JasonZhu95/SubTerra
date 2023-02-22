using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHitbox : WeaponComponent<ActionHitboxData, AttackActionHitbox>
{
    public event Action<Collider2D[]> OnDetectedCollider2D;

    private CoreComp<Movement> movement;

    private Vector2 offset;

    private Collider2D[] detected;

    private void HandleAttackAction()
    {
        offset.Set(
            transform.position.x + (currentAttackData.Hitbox.center.x * movement.Comp.FacingDirection),
            transform.position.y + currentAttackData.Hitbox.center.y
            );

        detected = Physics2D.OverlapBoxAll(offset, currentAttackData.Hitbox.size, 0f, data.DetectableLayers);

        if (detected.Length == 0)
            return;

        OnDetectedCollider2D?.Invoke(detected);
    }

    protected override void Start()
    {
        base.Start();

        movement = new CoreComp<Movement>(Core);
        eventHandler.OnAttackAction += HandleAttackAction;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        eventHandler.OnAttackAction -= HandleAttackAction;
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null)
            return;

        foreach(var item in data.AttackData)
        {
            if (!item.Debug)
                continue;

            Gizmos.DrawWireCube(transform.position + (Vector3)item.Hitbox.center, item.Hitbox.size);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : WeaponComponent<DamageData, AttackDamage>
{
    private ActionHitbox hitbox;

    private void HandleDetectCollider2D(Collider2D[] colliders)
    {
        foreach (var item in colliders)
        {
            if (item.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(currentAttackData.Amount);
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        hitbox = GetComponent<ActionHitbox>();
        hitbox.OnDetectedCollider2D += HandleDetectCollider2D;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        hitbox.OnDetectedCollider2D -= HandleDetectCollider2D;
    }
}

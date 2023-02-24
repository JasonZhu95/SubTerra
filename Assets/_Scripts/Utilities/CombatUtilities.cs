using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatUtilities
{
    #region IDamageable Check

    public static bool CheckIfDamageable(GameObject obj, DamageData data, out IDamageable damageable)
    {
        if (!obj.TryGetComponentInChildren(out damageable)) return false;
        damageable.Damage(data);
        return true;
    }

    public static bool CheckIfDamageable(Collider2D obj, DamageData data, out IDamageable damageable)
    {
        return CheckIfDamageable(obj.gameObject, data, out damageable);
    }

    public static bool CheckIfDamageable(RaycastHit2D obj, DamageData data, out IDamageable damageable)
    {
        return CheckIfDamageable(obj.collider, data, out damageable);
    }

    #endregion
}


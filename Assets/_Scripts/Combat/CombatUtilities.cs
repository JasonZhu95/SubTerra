using Project.Combat.Interfaces;
using Project.ExtensionMethods;
using UnityEngine;
using ParryData = Project.Combat.Interfaces.ParryData;

namespace Project.Combat
{
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

        #region IKnockbackable Check

        public static bool CheckIfKnockbackable(GameObject obj, KnockbackData data, out IKnockbackable knockbackable)
        {
            if (obj.TryGetComponentInChildren(out knockbackable))
            {
                knockbackable.Knockback(data);
                return true;
            }

            return false;
        }

        public static bool CheckIfKnockbackable(Collider2D obj, KnockbackData data, out IKnockbackable knockbackable) =>
            CheckIfKnockbackable(obj.gameObject, data, out knockbackable);

        public static bool CheckIfKnockbackable(RaycastHit2D obj, KnockbackData data, out IKnockbackable knockbackable) =>
            CheckIfKnockbackable(obj.collider, data, out knockbackable);

        #endregion

        #region IPoiseDamageable Check

        public static bool CheckIfPoiseDamageable(GameObject obj, PoiseDamageData data, out IPoiseDamageable poiseDamageable)
        {
            if (obj.TryGetComponent(out poiseDamageable))
            {
                poiseDamageable.PoiseDamage(data);
                return true;
            }

            return false;
        }

        public static bool CheckIfPoiseDamageable(Collider2D col, PoiseDamageData data, out IPoiseDamageable poiseDamageable) =>
            CheckIfPoiseDamageable(col.gameObject, data, out poiseDamageable);

        public static bool CheckIfPoiseDamageable(RaycastHit2D hit, PoiseDamageData data, out IPoiseDamageable poiseDamageable) =>
            CheckIfPoiseDamageable(hit.collider, data, out poiseDamageable);

        #endregion

        #region IParryable Check

        public static bool CheckIfParryable(GameObject obj, ParryData data, out IParryable parryable)
        {
            if (obj.TryGetComponent(out parryable))
            {
                parryable.Parry(data);
                return true;
            }

            return false;
        }

        public static bool CheckIfParryable(Collider2D col, ParryData data, out IParryable parryable) =>
            CheckIfParryable(col.gameObject, data, out parryable);

        public static bool CheckIfParryable(RaycastHit2D hit, ParryData data, out IParryable parryable) =>
            CheckIfParryable(hit.collider, data, out parryable);

        #endregion

        #region Other

        public static float AngleFromFacingDirection(Transform receiver, Transform source, int direction)
        {
            return Vector2.SignedAngle(Vector2.right * direction,
                source.position - receiver.position) * direction;
        }

        #endregion
    }
}
using UnityEngine;
using Project.Combat;
using Project.Utilities;
using Project.Combat.Interfaces;

namespace Project.Projectiles
{
    public class ProjectileDamage : ProjectileComponent<ProjectileDamageData>
    {
        private IHitbox[] hitboxes = new IHitbox[0];

        private DamageData damageData;

        public override void SetReferences()
        {
            base.SetReferences();

            hitboxes = GetComponents<IHitbox>();

            Data = Projectile.Data.GetComponentData<ProjectileDamageData>();

            damageData.SetData(gameObject, Data.DamageAmount);

            OnEnable();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            foreach (IHitbox hitbox in hitboxes)
            {
                hitbox.OnDetected += CheckHits;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (IHitbox hitbox in hitboxes)
            {
                hitbox.OnDetected -= CheckHits;
            }
        }

        private void CheckHits(RaycastHit2D[] hits)
        {
            print($"Found {hits.Length} hit(s)");
            if (!Projectile.CanDamage) return;
            foreach (var hit in hits)
            {
                print($"hit: {hit.transform.name}");
                if (!LayerMaskUtilities.IsLayerInLayerMask(hit, Data.LayerMask)) continue;
                if (CombatUtilities.CheckIfDamageable(hit, damageData, out _))
                {
                    Projectile.Disable();
                }
            }
        }
    }

    public class ProjectileDamageData : ProjectileComponentData
    {
        public float DamageAmount;
        public LayerMask LayerMask;

        public ProjectileDamageData()
        {
            ComponentDependencies.Add(typeof(ProjectileDamage));
        }
    }
}
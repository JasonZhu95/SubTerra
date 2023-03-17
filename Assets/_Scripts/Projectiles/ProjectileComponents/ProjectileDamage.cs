using UnityEngine;
using Project.Combat;
using Project.Utilities;
using Project.Interfaces;

namespace Project.Projectiles
{
    public class ProjectileDamage : ProjectileComponent<ProjectileDamageData>
    {
        private IHitbox[] hitboxes = new IHitbox[0];

        private DamageData damageData;
        private TriggerableData triggerData;

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
            if (!Projectile.CanDamage) return;
            foreach (var hit in hits)
            {
                triggerData.SetData(hit.collider.gameObject, true);
                if (!LayerMaskUtilities.IsLayerInLayerMask(hit, Data.LayerMask)) continue;
                if (CombatUtilities.CheckIfDamageable(hit, damageData, out _))
                {
                    Projectile.Disable();
                }
                CombatUtilities.CheckIfTriggerable(hit, triggerData, out _);
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
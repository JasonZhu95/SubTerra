using UnityEngine;
using Project.Utilities;
using Project.Combat.Interfaces;

namespace Project.Projectiles
{
    public class DespawnOnTouchLayer : ProjectileComponent<DespawnOnTouchLayerData>
    {
        private IHitbox[] hitboxes;

        public override void SetReferences()
        {
            base.SetReferences();

            hitboxes = GetComponents<IHitbox>();

            foreach (IHitbox hitbox in hitboxes)
            {
                hitbox.OnDetected += HandleDetected;
            }
        }

        private void HandleDetected(RaycastHit2D[] hits)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (LayerMaskUtilities.IsLayerInLayerMask(hit.collider.gameObject.layer, Data.Layer))
                {
                    Projectile.Disable();
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (IHitbox hitbox in hitboxes)
            {
                hitbox.OnDetected -= HandleDetected;
            }
        }
    }

    public class DespawnOnTouchLayerData : ProjectileComponentData
    {
        [field: SerializeField] public LayerMask Layer { get; private set; }

        public DespawnOnTouchLayerData()
        {
            ComponentDependencies.Add(typeof(DespawnOnTouchLayer));
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapons
{
    public class KnockbackOnHitboxAction : WeaponKnockback<KnockbackOnHitboxActionData>
    {
        private WeaponActionHitbox hitbox;

        private WeaponKnockbackStruct currentAttackData;

        protected override void SetCurrentAttackData()
        {
            base.SetCurrentAttackData();
            currentAttackData = data.GetAttackData(counter);
        }

        public override void SetReferences()
        {
            base.SetReferences();
            hitbox = GetComponent<WeaponActionHitbox>();
        }

        private void HandleDetected(Collider2D[] detected)
        {
            foreach (var item in detected)
            {
                CheckKnockback(item.gameObject, currentAttackData);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            hitbox.OnDetected += HandleDetected;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            hitbox.OnDetected -= HandleDetected;
        }
    }

    public class KnockbackOnHitboxActionData : WeaponComponentData<WeaponKnockbackStruct>
    {
        public KnockbackOnHitboxActionData()
        {
            ComponentDependencies.Add(typeof(KnockbackOnHitboxAction));
        }
    }
}

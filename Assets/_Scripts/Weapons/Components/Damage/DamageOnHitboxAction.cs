using UnityEngine;

namespace Project.Weapons
{
    public class DamageOnHitboxAction : WeaponDamage<DamageOnHitboxActionData>
    {
        private WeaponActionHitbox hitbox;

        private WeaponDamageStruct currentAttackData;

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
                CheckDamage(item.gameObject, currentAttackData);
                CheckTrigger(item.gameObject, true);
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

    public class DamageOnHitboxActionData : WeaponComponentData<WeaponDamageStruct>
    {
        public DamageOnHitboxActionData()
        {
            ComponentDependencies.Add(typeof(DamageOnHitboxAction));
        }
    }
}
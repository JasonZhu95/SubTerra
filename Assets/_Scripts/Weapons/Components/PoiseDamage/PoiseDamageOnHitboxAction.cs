using Project.Combat;
using Project.Interfaces;
using Project.Weapons.PoiseDamage;
using UnityEngine;

namespace Project.Weapons
{
    public class PoiseDamageOnHitboxAction : WeaponPoiseDamage<PoiseDamageOnHitboxActionData>
    {
        private WeaponActionHitbox hitbox;

        private WeaponPoiseDamageStruct currentAttackData;

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
                CheckPoiseDamage(item.gameObject, currentAttackData);
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

    public class PoiseDamageOnHitboxActionData : WeaponComponentData<WeaponPoiseDamageStruct>
    {
        public PoiseDamageOnHitboxActionData()
        {
            ComponentDependencies.Add(typeof(PoiseDamageOnHitboxAction));
        }
    }
}
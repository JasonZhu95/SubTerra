using Project.Combat;
using UnityEngine;

namespace Project.Weapons
{
    public class KnockbackOnParried : WeaponKnockback<KnockbackOnParriedData>
    {
        private Parry parry;

        private WeaponKnockbackStruct currentAttackData;

        private void HandleParry(GameObject parriedObj)
        {
            CheckKnockback(parriedObj, currentAttackData);
        }
        
        public override void SetReferences()
        {
            base.SetReferences();
            
            parry = GetComponent<Parry>();
        }

        protected override void SetCurrentAttackData()
        {
            base.SetCurrentAttackData();
            currentAttackData = data.GetAttackData(counter);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            parry.OnParry += HandleParry;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            parry.OnParry -= HandleParry;
        }
    }

    public class KnockbackOnParriedData : WeaponComponentData<WeaponKnockbackStruct>
    {
        public KnockbackOnParriedData()
        {
            ComponentDependencies.Add(typeof(KnockbackOnParried));
        }
    }
}
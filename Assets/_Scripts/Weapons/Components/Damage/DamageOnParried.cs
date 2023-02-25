using UnityEngine;

namespace Project.Weapons
{
    public class DamageOnParried : WeaponDamage<DamageOnParriedData>
    {
        private Parry parry;

        private WeaponDamageStruct currentAttackData;

        private void HandleParry(GameObject parriedObj)
        {
            CheckDamage(parriedObj, currentAttackData);
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
    }

    public class DamageOnParriedData : WeaponComponentData<WeaponDamageStruct>
    {
        public DamageOnParriedData()
        {
            ComponentDependencies.Add(typeof(DamageOnParried));
        }
    }
}
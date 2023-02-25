using Project.CoreComponents;
using UnityEngine;

namespace Project.Weapons
{
    public class DamageOnBlock : WeaponDamage<DamageOnBlockData>
    {
        private Block block;

        private WeaponDamageStruct currentAttackData;

        private void HandleBlock(GameObject blockedObj)
        {
            CheckDamage(blockedObj, currentAttackData);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            block.OnBlock += HandleBlock;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            block.OnBlock -= HandleBlock;
        }

        public override void SetReferences()
        {
            base.SetReferences();
            
            block = GetComponent<Block>();
        }

        protected override void SetCurrentAttackData()
        {
            base.SetCurrentAttackData();

            currentAttackData = data.GetAttackData(counter);
        }
    }
    
    public class DamageOnBlockData : WeaponComponentData<WeaponDamageStruct>
    {
        public DamageOnBlockData()
        {
            ComponentDependencies.Add(typeof(DamageOnBlock));
        }
    }
}
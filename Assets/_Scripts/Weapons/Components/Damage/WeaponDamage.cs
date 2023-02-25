using UnityEngine;
using Project.Combat;
using Project.Combat.Interfaces;

namespace Project.Weapons
{
    public class WeaponDamage<T> : WeaponComponent<T> where T : WeaponComponentData
    {
        private DamageData damageData;

        protected void CheckDamage(GameObject obj, WeaponDamageStruct data)
        {
            damageData.SetData(core.Parent, data.DamageAmount);
            CombatUtilities.CheckIfDamageable(obj, damageData, out _);
        }
    }

    [System.Serializable]
    public struct WeaponDamageStruct : INameable
    {
        [HideInInspector] public string AttackName;

        [field: SerializeField] public float DamageAmount { get; private set; }

        public void SetName(string value) => AttackName = value;
    }
}

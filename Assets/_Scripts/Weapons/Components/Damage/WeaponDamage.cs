using UnityEngine;
using Project.Combat;
using Project.Interfaces;

namespace Project.Weapons
{
    public class WeaponDamage<T> : WeaponComponent<T> where T : WeaponComponentData
    {
        private DamageData damageData;
        private TriggerableData triggerData;

        protected void CheckDamage(GameObject obj, WeaponDamageStruct data)
        {
            damageData.SetData(core.Parent, data.DamageAmount);
            CombatUtilities.CheckIfDamageable(obj, damageData, out _);
        }

        protected void CheckTrigger(GameObject obj, bool setTrigger)
        {
            triggerData.SetData(obj, setTrigger);
            CombatUtilities.CheckIfTriggerable(obj, triggerData, out _);
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

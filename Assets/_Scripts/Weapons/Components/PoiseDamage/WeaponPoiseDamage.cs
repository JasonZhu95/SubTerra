using System;
using Project.Combat;
using Project.Interfaces;
using UnityEngine;

namespace Project.Weapons.PoiseDamage
{
    public abstract class WeaponPoiseDamage<T> : WeaponComponent<T> where T : WeaponComponentData
    {
        private PoiseDamageData poiseDamageData;

        protected void CheckPoiseDamage(GameObject obj, WeaponPoiseDamageStruct data)
        {
            poiseDamageData.SetData(core.Parent, data.PoiseDamageAmount);

            CombatUtilities.CheckIfPoiseDamageable(obj, poiseDamageData, out _);
        }
    }

    [Serializable]
    public struct WeaponPoiseDamageStruct : INameable
    {
        [HideInInspector] public string attackName;

        [field: SerializeField] public float PoiseDamageAmount { get; private set; }

        public void SetName(string value)
        {
            attackName = value;
        }
    }
}
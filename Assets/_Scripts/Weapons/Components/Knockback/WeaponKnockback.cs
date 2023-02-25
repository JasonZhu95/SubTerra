using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Project.Combat;

namespace Project.Weapons
{
    public class WeaponKnockback<T> : WeaponComponent<T> where T : WeaponComponentData
    {
        private Movement Movement => movement ?? core.GetCoreComponent(ref movement);
        private Movement movement;

        protected void CheckKnockback(GameObject obj, WeaponKnockbackStruct data)
        {
            var knockbackData = new KnockbackData(data.KnockbackAngle, data.KnockbackStrength,
                Movement.FacingDirection, gameObject);

            CombatUtilities.CheckIfKnockbackable(obj, knockbackData, out _);
        }
    }

    [Serializable]
    public struct WeaponKnockbackStruct : INameable
    {
        [HideInInspector] public string attackName;

        [field: SerializeField] public Vector2 KnockbackAngle { get; private set; }
    
        [field: SerializeField] public float KnockbackStrength { get; private set; }

        public void SetName(string value) => attackName = value;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapons
{
    public class WeaponComponentModifier<T> : WeaponComponent<T> where T: WeaponComponentData
    {
        protected WeaponModifiers modifiers;

        public override void SetReferences()
        {
            base.SetReferences();
            modifiers = GetComponent<WeaponModifiers>();
        }
    }
}

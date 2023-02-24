using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponentModifier<T> : WeaponComponent<T> where T: WeaponComponentData
{
    protected WeaponModifiers modifiers;

    public override void SetReferences()
    {
        base.SetReferences();
        modifiers = GetComponent<WeaponModifiers>();
    }
}

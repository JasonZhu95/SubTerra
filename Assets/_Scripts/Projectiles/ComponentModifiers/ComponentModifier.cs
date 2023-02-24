using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentModifier<T> : ProjectileComponent<T> where T : ProjectileComponentData
{
    protected ProjectileModifiers modifiers;
    protected bool isActive;

    public override void SetReferences()
    {
        base.SetReferences();

        isActive = TryGetComponent(out modifiers);
    }
}
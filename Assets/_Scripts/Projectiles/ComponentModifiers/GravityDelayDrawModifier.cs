using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityDelayDrawModifier : ComponentModifier<GravityDelayDrawModifierData>
{
    private DelayedGravity delayedGravity;
    private DrawModifier modifier;

    public override void SetReferences()
    {
        base.SetReferences();

        if (TryGetComponent(out delayedGravity))
        {
            delayedGravity.OnSetDelay += ModifyDelay;
        }
    }

    private float ModifyDelay(float delay)
    {
        if (!isActive) return delay;

        if (modifiers.TryGetModifier(out modifier))
        {
            return delay * modifier.ModifierValue;
        }

        return delay;
    }

    protected override void OnEnable()
    {
        if (!delayedGravity) return;
        delayedGravity.OnSetDelay += ModifyDelay;
    }

    protected override void OnDisable()
    {
        if (!delayedGravity) return;
        delayedGravity.OnSetDelay -= ModifyDelay;
    }
}

public class GravityDelayDrawModifierData : ProjectileComponentData
{
    public GravityDelayDrawModifierData()
    {
        ComponentDependencies.Add(typeof(GravityDelayDrawModifier));
    }
}
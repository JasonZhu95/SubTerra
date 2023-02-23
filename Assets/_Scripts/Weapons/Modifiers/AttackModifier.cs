using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class AttackModifier { }

[System.Serializable]
public class AttackModifier<T> : AttackModifier
{
    public T ModifierValue { get; set; }
}

[System.Serializable]
public class DrawModifier : AttackModifier<float> { }

[System.Serializable]
public class ChargeModifier : AttackModifier<int> { }

[System.Serializable]
public class AllTargetModifier : AttackModifier<Collider2D[]> { }

[System.Serializable]
public class AccessibleTargetModifier : AttackModifier<Collider2D[]> { }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AttackActionHitbox : AttackData
{
    public bool Debug;

    [field: SerializeField] public Rect Hitbox { get; private set; }
}

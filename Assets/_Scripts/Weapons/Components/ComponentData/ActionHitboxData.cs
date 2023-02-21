using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionHitboxData : ComponentData<AttackActionHitbox>
{
    [field: SerializeField] public LayerMask DetectableLayers { get; private set; }
}

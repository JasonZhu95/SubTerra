using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementData : ComponentData<AttackMovement>
{
    public MovementData()
    {
        ComponentDependency = typeof(WeaponMovement);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Projectiles;

[CreateAssetMenu(fileName = "newRangedAttackStateData", menuName = "Data/State Data/Ranged Attack State")]
public class D_RangedAttackState : ScriptableObject
{
    public GameObject projectile;
    public ProjectileDataSO projectileData;
}

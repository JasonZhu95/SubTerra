using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponProjectileSpawnPoint
{
    public Vector2 offset;
    public Vector2 direction;
    public ProjectileDataSO projectileData;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Projectiles;

[CreateAssetMenu(fileName = "newArrowRainStateData", menuName = "Data/State Data/Arrow Rain State")]
public class D_ArrowRainState : ScriptableObject
{
    public float number_of_arrows = 10f;
    public float xVariation = 100f;
    public float delayMin = 0.1f;
    public float delayMax = 0.4f;
    public float spawnHeightOffset = 15f;
    public float warningHeightOffset = 10f;

    public GameObject arrowRainIndicator;
    public GameObject projectile;
    public ProjectileDataSO projectileData;
}
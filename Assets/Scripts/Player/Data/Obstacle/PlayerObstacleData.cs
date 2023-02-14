using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerObstacleData", menuName = "Data/ObstacleData")]
public class PlayerObstacleData : ScriptableObject
{
    [Header("Trampoline")]
    public float trampolineVelocity = 20f;
}

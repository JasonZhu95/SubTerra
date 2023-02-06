using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scriptable object where variables of the player is held
[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float movementVelocity = 10f;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newBeamAttackStateData", menuName = "Data/State Data/Beam Attack State")]
public class D_BeamAttackState : ScriptableObject
{
    public float beamDamage = 10f;
    public float poiseDamage = 0f;
    public float knockbackStrength;

    public Vector2 knockbackAngle;
    public Vector2 beamDimensions;

    public LayerMask whatIsPlayer;
}

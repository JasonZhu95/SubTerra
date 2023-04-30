using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "newDashFlurryStateData", menuName = "Data/State Data/Dash Flurry State")]
public class D_DashFlurryState : ScriptableObject
{
    public LayerMask whatIsPlayer;

    public Vector2 knockbackAngle;
    
    public float attackRadius = 0.3f;
    public float attackDamage = 1f;
    public float knockbackStrength = 10f;
    public float poiseDamage = 0f;
}

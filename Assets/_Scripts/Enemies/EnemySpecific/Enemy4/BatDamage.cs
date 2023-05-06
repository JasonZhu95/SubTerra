using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;

public class BatDamage : MonoBehaviour, IDamageable
{
    [SerializeField]
    private Enemy4 enemy4;

    public void Damage(DamageData data)
    {
        enemy4.TakeDamage(data.DamageAmount);
    }
}

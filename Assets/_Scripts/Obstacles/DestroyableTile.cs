using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;

public class DestroyableTile : MonoBehaviour, IDamageable
{
    private Animator tileAnim;
    private int numberOfTimesHit = 0;

    public void Damage(DamageData data)
    {
        numberOfTimesHit++;
        tileAnim.SetInteger("DamageCounter", numberOfTimesHit);
    }

    private void Awake()
    {
        tileAnim = GetComponent<Animator>();
    }

    public void DestroyTile()
    {
        Destroy(gameObject);
    }
}

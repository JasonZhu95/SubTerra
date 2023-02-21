using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageReceiver : CoreComponent, IDamageable
{
    private CoreComp<Stats> stats;
    private CoreComp<ParticleManager> particleManager;

    [SerializeField] private GameObject damageParticles;

    public void Damage(float amount)
    {
        Debug.Log(core.transform.parent.name + " Damaged");
        stats.Comp?.DecreaseHealth(amount);
        particleManager.Comp?.StartParticlesWithRandomRotation(damageParticles);
    }

    protected override void Awake()
    {
        base.Awake();

        stats = new CoreComp<Stats>(core);
        particleManager = new CoreComp<ParticleManager>(core);
    }
}

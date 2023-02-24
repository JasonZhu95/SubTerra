using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticlesOnDisable : ProjectileComponent<ProjectileParticlesOnDisableData>
{
    private Transform particleContainer;

    public override void SetReferences()
    {
        base.SetReferences();

        particleContainer = GameObject.FindGameObjectWithTag("ParticleContainer").transform;

        Projectile.OnBeforeDisable += StartParticles;
    }

    private void StartParticles()
    {
        Instantiate(Data.DespawnParticles, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)), particleContainer);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Projectile.OnBeforeDisable -= StartParticles;
    }
}

public class ProjectileParticlesOnDisableData : ProjectileComponentData
{
    [field: SerializeField] public GameObject DespawnParticles { get; private set; }

    public ProjectileParticlesOnDisableData()
    {
        ComponentDependencies.Add(typeof(ProjectileParticlesOnDisable));
    }
}

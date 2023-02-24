using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticles : ProjectileComponent<ProjectileParticlesData>
{
    private Transform particleContainer;

    private GameObject particles;
    private ParticleSystem particlesScript;

    protected override void Init()
    {
        base.Init();

        Data = Projectile.Data.GetComponentData<ProjectileParticlesData>();

        particles = Instantiate(Data.trailPrefab, transform);
        particlesScript = particles.GetComponent<ParticleSystem>();
    }

    private void MoveParticles()
    {
        if (!particles)
            return;

        particlesScript.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        particles.transform.parent = particleContainer;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Projectile.OnBeforeDisable += MoveParticles;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Projectile.OnBeforeDisable -= MoveParticles;
    }

    protected override void Awake()
    {
        base.Awake();

        particleContainer = GameObject.FindGameObjectWithTag("ParticleContainer").transform;
    }
}

public class ProjectileParticlesData : ProjectileComponentData
{
    public ProjectileParticlesData()
    {
        ComponentDependencies.Add(typeof(ProjectileParticles));
    }

    [field: SerializeField] public GameObject trailPrefab { get; private set; }
}
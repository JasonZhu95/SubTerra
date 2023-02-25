using UnityEngine;

namespace Project.Projectiles
{
    public class Particles : ProjectileComponent<ParticlesData>
    {
        private Transform particleContainer;

        private GameObject particles;
        private ParticleSystem particlesScript;

        protected override void Init()
        {
            base.Init();

            Data = Projectile.Data.GetComponentData<ParticlesData>();

            particles = Instantiate(Data.trailPrefab, transform);
            particlesScript = particles.GetComponent<ParticleSystem>();
        }

        private void MoveParticles()
        {
            if (!particles) return;
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

    public class ParticlesData : ProjectileComponentData
    {
        public ParticlesData()
        {
            ComponentDependencies.Add(typeof(Particles));
        }

        [field: SerializeField] public GameObject trailPrefab { get; private set; }
    }
}

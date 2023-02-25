using UnityEngine;
using Project.Utilities.Notifiers;

namespace Project.Projectiles
{
    public class DespawnProjectileOnStick : ProjectileComponent<DespawnProjectileOnStickData>
    {
        private StickInEnvironment stickInEnvironment;
        private bool stickInEnvironmentFound;

        private TimerNotifier despawnTimer;

        public override void SetReferences()
        {
            base.SetReferences();

            if (stickInEnvironmentFound = TryGetComponent(out stickInEnvironment))
            {
                stickInEnvironment.OnStick += HandleStick;
            }
        }

        private void HandleStick()
        {
            despawnTimer = new TimerNotifier(Data.DespawnTime, false);
            despawnTimer.OnTimerDone += Despawn;
        }

        private void Despawn()
        {
            Projectile.Disable();
        }

        private void Update()
        {
            despawnTimer?.Tick();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (stickInEnvironmentFound) stickInEnvironment.OnStick -= HandleStick;

            if (despawnTimer != null)
            {
                despawnTimer.OnTimerDone -= Despawn;
            }
        }
    }

    public class DespawnProjectileOnStickData : ProjectileComponentData
    {
        [field: SerializeField] public float DespawnTime { get; private set; }

        public DespawnProjectileOnStickData()
        {
            ComponentDependencies.Add(typeof(DespawnProjectileOnStick));
        }
    }
}
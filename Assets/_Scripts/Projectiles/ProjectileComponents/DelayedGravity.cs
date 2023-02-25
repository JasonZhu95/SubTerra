using UnityEngine;
using System;
using Project.Utilities.Notifiers;

namespace Project.Projectiles
{
    public class DelayedGravity : ProjectileComponent<DelayedGravityData>
    {
        private Rigidbody2D rb;
        private TimerNotifier timerNotifier;
        private DistanceNotifier distanceNotifier;

        protected override void Init()
        {
            base.Init();

            Data = Projectile.Data.GetComponentData<DelayedGravityData>();

            SetDistanceDelay();
            SetTimeDelay();
        }

        private void SetTimeDelay()
        {
            switch (Data.TimeDelay)
            {
                case 0f:
                    rb.gravityScale = Data.Gravity;
                    break;
                case > 0f:
                    var delay = Data.TimeDelay;
                    var modDelay = OnSetDelay?.Invoke(delay);
                    timerNotifier = new TimerNotifier(
                        modDelay != null ? (float)modDelay : delay,
                        false
                    );
                    timerNotifier.OnTimerDone += () => rb.gravityScale = Data.Gravity;
                    break;
            }
        }
        public event Func<float, float> OnSetDelay;

        private void SetDistanceDelay()
        {
            switch (Data.DistanceDelay)
            {
                case 0f:
                    rb.gravityScale = Data.Gravity;
                    break;
                case > 0f:
                    var delay = Data.DistanceDelay;
                    var modDelay = OnSetDelay?.Invoke(delay);
                    distanceNotifier = new DistanceNotifier(
                        transform.position,
                        modDelay != null ? (float)modDelay : delay,
                        false,
                        true);
                    distanceNotifier.OnTarget += () => rb.gravityScale = Data.Gravity;
                    break;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            timerNotifier?.Tick();
            distanceNotifier?.Tick(transform.position);
        }

    }

    public class DelayedGravityData : ProjectileComponentData
    {
        public float Gravity = 4f;
        public float DistanceDelay = -1;
        public float TimeDelay = -1;

        public DelayedGravityData()
        {
            ComponentDependencies.Add(typeof(DelayedGravity));
            ComponentDependencies.Add(typeof(ProjectileModifiers));
        }
    }
}
using UnityEngine;
using System;
using Project.Interfaces;

namespace Project.Projectiles
{
    public class HitLine : ProjectileComponent<HitLineData>, IHitbox
    {
        public event Action<RaycastHit2D[]> OnDetected;

        private RaycastHit2D[] hits;

        protected override void Init()
        {
            base.Init();

            Data = Projectile.Data.GetComponentData<HitLineData>();

            if (!Data.DoInitialCheck) return;

            hits = Physics2D.LinecastAll(transform.position, Projectile.SpawningEntityPos, Data.LayerMask);

            if (hits.Length > 0) CheckHits();
        }

        private void FixedUpdate()
        {
            hits = Physics2D.RaycastAll(
                transform.position,
                transform.right,
                Projectile.RB.velocity.magnitude * Time.deltaTime,
                Data.LayerMask
            );

            if (hits.Length > 0) CheckHits();
        }

        private void CheckHits()
        {
            OnDetected?.Invoke(hits);
        }
    }

    public class HitLineData : ProjectileComponentData
    {
        public LayerMask LayerMask;
        public bool DoInitialCheck = true;

        public HitLineData()
        {
            ComponentDependencies.Add(typeof(HitLine));
        }
    }
}

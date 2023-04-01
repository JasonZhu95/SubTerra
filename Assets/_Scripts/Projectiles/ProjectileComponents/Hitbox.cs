using UnityEngine;
using System;
using Project.Interfaces;

namespace Project.Projectiles
{
    public class Hitbox : ProjectileComponent<HitboxData>, IHitbox
    {
        public event Action<RaycastHit2D[]> OnDetected;

        private RaycastHit2D[] hits;

        private Rigidbody2D rb;

        public override void SetReferences()
        {
            base.SetReferences();

            Data = Projectile.Data.GetComponentData<HitboxData>();
        }

        protected override void Init()
        {
            base.Init();

            if (!Data.DoInitialCheck) return;
            hits = Physics2D.LinecastAll(transform.position, Projectile.SpawningEntityPos, Data.LayerMask);

            if (hits.Length > 0) CheckHits();
        }

        private void FixedUpdate()
        {
            var dist = Data.CompensateForVelocity ? rb.velocity.magnitude * Time.deltaTime : 0;
            hits = Physics2D.BoxCastAll(
                transform.position + (Vector3)Data.Hitbox.center,
                Data.Hitbox.size,
                transform.rotation.eulerAngles.z,
                transform.right,
                dist,
                Data.LayerMask
            );

            if (hits.Length > 0) CheckHits();
        }

        private void CheckHits()
        {
            OnDetected?.Invoke(hits);
        }

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnDrawGizmos()
        {
            if (Data == null) return;

            Gizmos.DrawWireCube(
                transform.position + (Vector3)Data.Hitbox.center,
                Data.Hitbox.size);
        }
    }

    public class HitboxData : ProjectileComponentData
    {
        public Rect Hitbox;
        public bool CompensateForVelocity;
        public LayerMask LayerMask;
        public bool DoInitialCheck = true;

        public HitboxData()
        {
            ComponentDependencies.Add(typeof(Hitbox));
        }
    }
}
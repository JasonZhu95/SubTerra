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
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.name == "DestroyableTile")
                {
                    RaycastHit2D[] tempHit = new RaycastHit2D[hits.Length - 1];
                    if (i > 0)
                    {
                        Array.Copy(hits, 0, tempHit, 0, i);
                    }
                    if (i < hits.Length - 1)
                    {
                        Array.Copy(hits, i + 1, tempHit, i, hits.Length - i - 1);
                    }
                    hits = tempHit;
                }
            }

            if (hits.Length > 0) CheckHits();
        }

        private void FixedUpdate()
        {
            // Look for colliders in the hitbox of the arrow
            var dist = Data.CompensateForVelocity ? rb.velocity.magnitude * Time.deltaTime : 0;
            hits = Physics2D.BoxCastAll(
                transform.position + (Vector3)Data.Hitbox.center,
                Data.Hitbox.size,
                transform.rotation.eulerAngles.z,
                transform.right,
                dist,
                Data.LayerMask
            );
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.name == "DestroyableTile")
                {
                    RaycastHit2D[] tempHit = new RaycastHit2D[hits.Length - 1];
                    if (i > 0)
                    {
                        Array.Copy(hits, 0, tempHit, 0, i);
                    }
                    if (i < hits.Length - 1)
                    {
                        Array.Copy(hits, i + 1, tempHit, i, hits.Length - i - 1);
                    }
                    hits = tempHit;
                }
            }

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
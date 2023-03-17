using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Project.Interfaces;
using Project.Utilities;

namespace Project.Projectiles
{
    public class StickInEnvironment : ProjectileComponent<StickInEnvironmentData>
    {
        private Rigidbody2D rb;
        private SpriteRenderer sr;

        private bool isStuck;

        private IHitbox[] hitboxes;

        public event Action OnStick;

        public override void SetReferences()
        {
            base.SetReferences();

            hitboxes = GetComponents<IHitbox>();

            Data = Projectile.Data.GetComponentData<StickInEnvironmentData>();

            foreach (IHitbox hitbox in hitboxes)
            {
                hitbox.OnDetected += CheckHits;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (IHitbox hitbox in hitboxes)
            {
                hitbox.OnDetected -= CheckHits;
            }
        }

        private void CheckHits(RaycastHit2D[] hits)
        {
            if (isStuck)
                return;

            foreach (var hit in hits)
            {
                if (!LayerMaskUtilities.IsLayerInLayerMask(hit, Data.LayerMask))
                    continue;

                if (hit.collider.TryGetComponent(out TilemapRenderer hitSR))
                {
                    sr.sortingLayerID = hitSR.sortingLayerID;
                    sr.sortingOrder = -1;
                }

                var position = transform.position;
                var xDiff = position.x - position.x;
                var yDiff = position.y - position.y;

                Vector3 stickPos = new Vector3(hit.point.x - xDiff, hit.point.y - yDiff, 0f);

                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;

                position = stickPos;

                Debug.DrawRay(stickPos, Vector3.left, Color.red, 5f);

                transform.position = position;

                var currentRot = transform.rotation.eulerAngles.z;
                var randomRot = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(currentRot - 10f, currentRot + 10f));

                transform.rotation = randomRot;

                isStuck = true;

                OnStick?.Invoke();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            rb = GetComponent<Rigidbody2D>();
            sr = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public class StickInEnvironmentData : ProjectileComponentData
    {
        public LayerMask LayerMask;

        public StickInEnvironmentData()
        {
            ComponentDependencies.Add(typeof(StickInEnvironment));
        }
    }
}

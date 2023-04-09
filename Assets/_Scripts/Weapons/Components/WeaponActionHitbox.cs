using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Project.Weapons
{
    public class WeaponActionHitbox : WeaponComponent<WeaponHitboxData>
    {
        public event Action<Collider2D[]> OnDetected;

        private Vector2 offset;

        private Movement Movement { get => movement ?? core.GetCoreComponent(ref movement); }
        private Movement movement;

        private HitboxStruct currentAttackData;

        private Collider2D[] detected;
        private Collider2D[] detectedHammer;

        private void CheckHitbox()
        {
            // Set hitbox offset based on current position
            offset.Set(
              transform.position.x + (currentAttackData.Hitbox.position.x * Movement.FacingDirection),
              transform.position.y + currentAttackData.Hitbox.y
              );

            // Look for colliders in the hitbox
            detected = Physics2D.OverlapBoxAll(offset, currentAttackData.Hitbox.size, 0f, data.DamageableLayers);
            if (!currentAttackData.isHammer)
            {
                for (int i = 0; i < detected.Length; i++)
                {
                    if (detected[i].gameObject.name == "DestroyableTile")
                    {
                        detectedHammer = new Collider2D[detected.Length - 1];
                        if (i > 0)
                        {
                            Array.Copy(detected, 0, detectedHammer, 0, i);
                        }
                        if (i < detected.Length - 1)
                        {
                            Array.Copy(detected, i + 1, detectedHammer, i, detected.Length - i - 1);
                        }
                        detected = detectedHammer;
                    }
                }
            }
            if (detected.Length == 0) return;

            OnDetected?.Invoke(detected);
        }

        protected override void SetCurrentAttackData()
        {
            base.SetCurrentAttackData();
            currentAttackData = data.GetAttackData(counter);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            eventHandler.OnAttackAction += CheckHitbox;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            eventHandler.OnAttackAction -= CheckHitbox;
        }

    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var data = GetComponent<Weapon>().WeaponData?.GetComponentData<WeaponHitboxData>()?.GetAllData();

            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item.debug)
                    {
                        Gizmos.DrawWireCube(transform.position + (Vector3)item.Hitbox.position, (Vector3)item.Hitbox.size);
                    }
                }
            }
        }
    #endif
    }

    public class WeaponHitboxData : WeaponComponentData<HitboxStruct>
    {
        [field: SerializeField] public LayerMask DamageableLayers { get; private set; }

        public WeaponHitboxData() => ComponentDependencies.Add(typeof(WeaponActionHitbox));
    }

    [System.Serializable]
    public struct HitboxStruct : INameable
    {
        [HideInInspector]
        public string AttackName;

        public bool debug;
        [field: SerializeField]
        public Rect Hitbox { get; private set; }
        public bool isHammer;

        public void SetName(string value) => AttackName = value;
    }
}
using System;
using Project.Combat;
using UnityEngine;

namespace Project.Weapons
{
    public class Parry : WeaponComponent<ParryData>
    {
        public event Action<GameObject> OnParry;

        private ParryDataStruct currentAttackData;

        private float parryWindowStartTime;
        private float parryWindowEndTime;

        private bool shouldCheckParryTime;
        private bool isParryWindowActive;

        private Vector2 offset;

        private Collider2D[] detected;

        private Movement movement;
        private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);

        private ParticleManager particleManager;

        private ParticleManager ParticleManager =>
            particleManager ? particleManager : core.GetCoreComponent(ref particleManager);

        private void HandleEnter()
        {
            parryWindowStartTime = Time.time + currentAttackData.ParryWindowStart;
            parryWindowEndTime = Time.time + currentAttackData.ParryWindowEnd;
            shouldCheckParryTime = true;
        }

        private void Update()
        {
            if (!shouldCheckParryTime)
                return;

            if (!isParryWindowActive && Time.time >= parryWindowStartTime)
                EnableParryWindow();

            if (isParryWindowActive && Time.time >= parryWindowEndTime)
                DisableParryWindow();

            CheckParryHitbox();
        }

        private void CheckParryHitbox()
        {
            if (!isParryWindowActive) return;

            offset.Set(
                transform.position.x + (currentAttackData.ParryHitbox.position.x * Movement.FacingDirection),
                transform.position.y + currentAttackData.ParryHitbox.y
            );

            detected = Physics2D.OverlapBoxAll(offset, currentAttackData.ParryHitbox.size, 0f, data.ParryableLayer);

            if (detected.Length == 0) return;

            var parryData = new Interfaces.ParryData
            {
                source = gameObject
            };

            foreach (var item in detected)
            {
                if (CombatUtilities.CheckIfParryable(item, parryData, out var parryable))
                {
                    ParticleManager.StartParticlesWithRandomRotation(data.ParryParticlesPrefab,
                        data.ParryParticlesOffset);
                    OnParry?.Invoke(parryable.GetParent());
                    weapon.Anim.SetTrigger(WeaponTriggerAnimParameters.parry.ToString());
                    DisableParryWindow();
                }
            }
        }

        private void EnableParryWindow()
        {
            isParryWindowActive = true;
        }

        private void DisableParryWindow()
        {
            isParryWindowActive = false;
            shouldCheckParryTime = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            weapon.OnEnter += HandleEnter;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            weapon.OnEnter -= HandleEnter;
        }

        protected override void SetCurrentAttackData() => currentAttackData = data.GetAttackData(counter);
    }

    public class ParryData : WeaponComponentData<ParryDataStruct>
    {
        [field: SerializeField] public GameObject ParryParticlesPrefab { get; private set; }
        [field: SerializeField] public Vector2 ParryParticlesOffset { get; private set; }
        [field: SerializeField] public LayerMask ParryableLayer { get; private set; }

        public ParryData()
        {
            ComponentDependencies.Add(typeof(Parry));
        }
    }

    [Serializable]
    public struct ParryDataStruct : INameable
    {
        [HideInInspector] public string attackName;

        [field: SerializeField] public float DamageAbsorption { get; private set; }
        [field: SerializeField] public float ParryWindowStart { get; private set; }
        [field: SerializeField] public float ParryWindowEnd { get; private set; }
        [field: SerializeField] public Rect ParryHitbox { get; private set; }

        public void SetName(string value)
        {
            attackName = value;
        }
    }
}
using System;
using Project.CoreComponents;
using Project.Modifiers;
using UnityEngine;

namespace Project.Weapons
{
    public class Block : WeaponComponent<BlockData>
    {
        public event Action<GameObject> OnBlock;
        
        // Related Core Components
        private DamageComponent damageComponent;

        private DamageComponent DamageComponent =>
            damageComponent ? damageComponent : core.GetCoreComponent(ref damageComponent);

        private KnockbackComponent knockbackComponent;

        private KnockbackComponent KnockbackComponent =>
            knockbackComponent ? knockbackComponent : core.GetCoreComponent(ref knockbackComponent);

        private PoiseDamageComponent poiseDamageComponent;

        private PoiseDamageComponent PoiseDamageComponent => poiseDamageComponent
            ? poiseDamageComponent
            : core.GetCoreComponent(ref poiseDamageComponent);

        private Movement movement;
        private Movement Movement => movement ? movement : core.GetCoreComponent(ref movement);

        private ParticleManager particleManager;

        private ParticleManager ParticleManager =>
            particleManager ? particleManager : core.GetCoreComponent(ref particleManager);

        private BlockStruct currentAttackData;

        // Modifiers
        private BlockDamageModifier damageModifier;
        private BlockKnockbackModifier knockbackModifier;
        private BlockPoiseDamageModifier poiseDamageModifier;

        private bool isBlockWindowActive;
        private bool shouldCheckBlockTime;

        private void HandleEnterAttackPhase(WeaponAttackPhases phase)
        {
            if (!isBlockWindowActive)
            {
                shouldCheckBlockTime = currentAttackData.BlockWindowStart.SetTriggerTime(phase);
            }
            else
            {
                shouldCheckBlockTime = currentAttackData.BlockWindowEnd.SetTriggerTime(phase);
            }
        }

        private void Update()
        {
            if (!shouldCheckBlockTime)
                return;

            if (isBlockWindowActive)
            {
                if (currentAttackData.BlockWindowEnd.IsPastTriggerTime)
                    DisableBlockWindow();
            }
            else
            {
                if (currentAttackData.BlockWindowStart.IsPastTriggerTime)
                    EnableBlockWindow();
            }
        }

        private void EnableBlockWindow()
        {
            isBlockWindowActive = true;

            damageModifier.OnBlock += HandleSuccessfulBlock;

            DamageComponent.DamageModifiers.AddModifier(damageModifier);
            KnockbackComponent.KnockbackModifiers.AddModifier(knockbackModifier);
            PoiseDamageComponent.PoiseDamageModifiers.AddModifier(poiseDamageModifier);
            shouldCheckBlockTime = false;
        }

        private void DisableBlockWindow()
        {
            isBlockWindowActive = false;

            damageModifier.OnBlock -= HandleSuccessfulBlock;

            DamageComponent.DamageModifiers.RemoveModifier(damageModifier);
            KnockbackComponent.KnockbackModifiers.RemoveModifier(knockbackModifier);
            PoiseDamageComponent.PoiseDamageModifiers.RemoveModifier(poiseDamageModifier);
            shouldCheckBlockTime = false;
        }

        private void HandleSuccessfulBlock(GameObject blockedObj)
        {
            ParticleManager.StartParticlesWithRandomRotation(currentAttackData.BlockParticles,
                currentAttackData.BlockParticlesOffset);
            
            OnBlock?.Invoke(blockedObj);
        }

        private void Start()
        {
            damageModifier = new BlockDamageModifier(currentAttackData, Movement,
                core.EntityTransform);
            
            knockbackModifier =
                new BlockKnockbackModifier(currentAttackData, Movement, core.EntityTransform);

            poiseDamageModifier = new BlockPoiseDamageModifier(currentAttackData, Movement, core.EntityTransform);
        }


        protected override void SetCurrentAttackData()
        {
            base.SetCurrentAttackData();
            currentAttackData = data.GetAttackData(counter);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            eventHandler.OnEnterAttackPhase += HandleEnterAttackPhase;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            eventHandler.OnEnterAttackPhase -= HandleEnterAttackPhase;
        }
    }

    public class BlockData : WeaponComponentData<BlockStruct>
    {
        public BlockData()
        {
            ComponentDependencies.Add(typeof(Block));
        }
    }

    [System.Serializable]
    public struct BlockStruct : INameable
    {
        [HideInInspector] public string AttackName;

        [field: SerializeField] public PhaseTime BlockWindowStart { get; private set; }
        [field: SerializeField] public PhaseTime BlockWindowEnd { get; private set; }

        [field: SerializeField] public GameObject BlockParticles { get; private set; }
        [field: SerializeField] public Vector2 BlockParticlesOffset { get; private set; }
        [field: SerializeField] public BlockDirection[] BlockDirections { get; private set; }

        public void SetName(string value)
        {
            AttackName = value;
        }
    }

    [System.Serializable]
    public struct PhaseTime
    {
        [field: SerializeField] public float Duration { get; private set; }
        [field: SerializeField] public WeaponAttackPhases Phase { get; private set; }

        public float TriggerTime { get; private set; }

        public bool SetTriggerTime(WeaponAttackPhases phase)
        {
            if (phase != Phase)
                return false;
            TriggerTime = Time.time + Duration;
            return true;
        }

        public bool IsPastTriggerTime => Time.time >= TriggerTime;
    }

    [System.Serializable]
    public struct BlockDirection
    {
        [Range(-180f, 180f)] public float MinAngle;
        [Range(-180f, 180f)] public float MaxAngle;
        [Range(0f, 1f)] public float DamageAbsorption;
        [Range(0f, 1f)] public float KnockbackAbsorption;
        [Range(0f, 1f)] public float PoiseDamageAbsorption;

        public bool IsBetween(float angle)
        {
            if (MaxAngle > MinAngle)
            {
                return angle >= MinAngle && angle <= MaxAngle;
            }

            return (angle >= MinAngle && angle <= 180f) || (angle <= MaxAngle && angle >= -180f);
        }
    }
}
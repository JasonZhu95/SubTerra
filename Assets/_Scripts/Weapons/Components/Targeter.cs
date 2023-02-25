using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapons
{
    public class Targeter : WeaponComponent<TargeterData>
    {

        public event Action<Collider2D[]> OnFindAllTargets;
        public event Action<Collider2D[]> OnFindAccesibleTargets;

        private WeaponModifiers modifiers;

        private AllTargetModifier allTargetModifier = new AllTargetModifier();
        private AccessibleTargetModifier accessibleTargetModifier = new AccessibleTargetModifier();

        private TargeterShape currentAttackData;

        private Movement movement;
        public Movement Movement
        {
            get => movement ?? core.GetCoreComponent(ref movement);
            private set => movement = value;
        }

        private void DetermineAllTargets()
        {
            currentAttackData = data.GetAttackData(counter);

            var pos = transform.position +
                      new Vector3(currentAttackData.Offset.x * Movement.FacingDirection, currentAttackData.Offset.y);

            var targets = Physics2D.OverlapBoxAll(pos, currentAttackData.Size, 0f, currentAttackData.damageableLayer);
            
            OnFindAllTargets?.Invoke(targets);

            if(targets.Length == 0) return;
            
            allTargetModifier.ModifierValue = targets;
            modifiers.AddModifier(allTargetModifier);
            
             CheckTargetAccessability(targets);
        }

        private void CheckTargetAccessability(Collider2D[] targets)
        {
            List<Collider2D> accessabile = new List<Collider2D>();

            foreach (Collider2D target in targets)
            {
                var hit = Physics2D.Linecast(transform.position, target.transform.position,
                    currentAttackData.groundLayer);

                if (!hit)
                {
                    accessabile.Add(target);
                }
            }

            var accessibileTargets = accessabile.ToArray();

            accessibleTargetModifier.ModifierValue = accessibileTargets;
            modifiers.AddModifier(accessibleTargetModifier);
            
            OnFindAccesibleTargets?.Invoke(accessibileTargets);
        }

        public override void SetReferences()
        {
            base.SetReferences();

            modifiers = GetComponent<WeaponModifiers>();
        }

        private void HandleInput(bool value)
        {
            if(!value) DetermineAllTargets();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            weapon.OnInputChange += HandleInput;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            weapon.OnInputChange -= HandleInput;
        }

        private void OnDrawGizmos()
        {
            if (!TryGetComponent(out Weapon wep)) return;
            if (!wep.WeaponData) return;
            var Data = wep.WeaponData.GetComponentData<TargeterData>();
            if (Data == null) return;
            var allData = Data.GetAllData();
            if (allData == null) return;

            foreach (TargeterShape shape in allData)
            {
                if(!shape.debug) continue;
                Gizmos.DrawWireCube(transform.position + (Vector3)shape.Offset, shape.Size);
            }
        }
    }
    
    public class TargeterData : WeaponComponentData<TargeterShape>
    {
        public TargeterData()
        {
            ComponentDependencies.Add(typeof(Targeter));
            ComponentDependencies.Add(typeof(WeaponModifiers));
        }
    }
}
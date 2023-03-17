using System;
using Project.Combat;
using Project.Interfaces;
using Project.Weapons;
using UnityEngine;

namespace Project.Modifiers
{
    public class BlockDamageModifier : DamageModifier
    {
        private readonly BlockStruct blockStruct;
        private readonly Movement movement;
        private readonly Transform transform;

        public event Action<GameObject> OnBlock;

        public BlockDamageModifier(BlockStruct blockStruct, Movement movement, Transform transform)
        {
            this.blockStruct = blockStruct;
            this.movement = movement;
            this.transform = transform;
        }

        public override DamageData ModifyValue(DamageData value)
        {
            foreach (var blockDirection in blockStruct.BlockDirections)
            {
                if (!blockDirection.IsBetween(DetermineDamageSourceDirection(value.Source))) continue;
                value.DamageAmount *= 1 - blockDirection.DamageAbsorption;
                OnBlock?.Invoke(value.Source);
                break;
            }

            return value;
        }

        private float DetermineDamageSourceDirection(GameObject source)
        {
            return CombatUtilities.AngleFromFacingDirection(transform, source.transform, movement.FacingDirection);
        }
    }
}
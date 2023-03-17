using Project.Combat;
using Project.Interfaces;
using Project.Weapons;
using UnityEngine;

namespace Project.Modifiers
{
    public class BlockPoiseDamageModifier : PoiseDamageModifier
    {
        private readonly BlockStruct blockStruct;
        private readonly Movement movement;
        private readonly Transform entityTransform;

        public BlockPoiseDamageModifier(BlockStruct blockStruct, Movement movement, Transform entityTransform)
        {
            this.blockStruct = blockStruct;
            this.movement = movement;
            this.entityTransform = entityTransform;
        }

        public override PoiseDamageData ModifyValue(PoiseDamageData value)
        {
            foreach (var blockDirection in blockStruct.BlockDirections)
            {
                if (!blockDirection.IsBetween(CombatUtilities.AngleFromFacingDirection(entityTransform, value.Source.transform, movement.FacingDirection)))
                    continue;
                value.PoiseDamageAmount *= 1 - blockDirection.PoiseDamageAbsorption;
            }

            return value;
        }
    }
}
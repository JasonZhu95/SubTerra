using Project.Combat;
using Project.Weapons;
using UnityEngine;

namespace Project.Modifiers
{
    public class BlockKnockbackModifier : KnockbackModifiers
    {
        private BlockStruct blockStruct;
        private Movement movement;
        private Transform transform;

        public BlockKnockbackModifier(BlockStruct blockStruct, Movement movement, Transform transform)
        {
            this.blockStruct = blockStruct;
            this.movement = movement;
            this.transform = transform;
        }

        public override KnockbackData ModifyValue(KnockbackData value)
        {
            foreach (var blockDirection in blockStruct.BlockDirections)
            {
                if (blockDirection.IsBetween(DetermineDamageSourceDirection(value.Source)))
                {
                    value.Strength *= 1 - blockDirection.KnockbackAbsorption;
                    break;
                }
            }

            return value;
        }

        private float DetermineDamageSourceDirection(GameObject source)
        {
            return CombatUtilities.AngleFromFacingDirection(transform, source.transform, movement.FacingDirection);
        }
    }
}
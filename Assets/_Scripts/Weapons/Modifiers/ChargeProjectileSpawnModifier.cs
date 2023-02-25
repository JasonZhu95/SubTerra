using UnityEngine;
using Project.Utilities;

namespace Project.Weapons
{
    public class ChargeProjectileSpawnModifier : WeaponComponentModifier<ChargeProjectileSpawnModifierData>
    {
        private AttackRanged ranged;
        private ChargeModifier modifier;

        public override void SetReferences()
        {
            base.SetReferences();
            ranged = GetComponent<AttackRanged>();
        }

        private int ModifyNumberOfProjectiles(int initVal)
        {
            if (modifiers.TryGetModifier(out modifier))
            {
                return initVal * modifier.ModifierValue;
            }

            return initVal;
        }

        private Vector2[] ModifyDirections(Vector2 originalDir)
        {


            if (modifiers.TryGetModifier(out modifier))
            {
                Vector2[] directions = new Vector2[modifier.ModifierValue];

                var currentAttackData = data.GetAttackData(counter);

                var originalAngle = VectorUtilities.AngleFromVector2(originalDir);

                var minAngle = originalAngle - currentAttackData.AngleVariation;
                var maxAngle = originalAngle + currentAttackData.AngleVariation;

                if (modifier.ModifierValue - 1 == 0) return new Vector2[] { originalDir }; ;

                var angleDiff = (maxAngle - minAngle) / (modifier.ModifierValue - 1);

                for (int i = 0; i < modifier.ModifierValue; i++)
                {
                    var ang = minAngle + (angleDiff * i);
                    directions[i] = VectorUtilities.Vector2FromAngle(ang);
                }

                return directions;
            }

            return new Vector2[] { originalDir };
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!modifiers || !ranged) return;

            ranged.OnSetNumberOfProjectiles += ModifyNumberOfProjectiles;
            ranged.OnSetProjectileDirection += ModifyDirections;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!modifiers || !ranged) return;

            ranged.OnSetNumberOfProjectiles -= ModifyNumberOfProjectiles;
            ranged.OnSetProjectileDirection -= ModifyDirections;
        }
    }

    public class ChargeProjectileSpawnModifierData : WeaponComponentData<ChargeProjectileSpawnModifierStruct>
    {
        public ChargeProjectileSpawnModifierData()
        {
            ComponentDependencies.Add(typeof(ChargeProjectileSpawnModifier));
            ComponentDependencies.Add(typeof(WeaponModifiers));
        }
    }
}
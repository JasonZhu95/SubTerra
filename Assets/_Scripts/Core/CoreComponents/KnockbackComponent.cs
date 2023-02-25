using Project.Modifiers;
using Project.Weapons;
using UnityEngine;

namespace Project.CoreComponents
{
    public class KnockbackComponent : CoreComponent, IKnockbackable
    {
        [SerializeField] private float maxKnockbackTime = 0.2f;

        private Movement Movement => movement ?? core.GetCoreComponent(ref movement);
        private Movement movement;

        private CollisionSenses collisionSenses;
        private CollisionSenses CollisionSenses => collisionSenses ?? core.GetCoreComponent(ref collisionSenses);

        private bool isKnockbackActive;
        private float knockbackStartTime;

        public ModifierContainer<KnockbackModifiers, KnockbackData> KnockbackModifiers { get; private set; } =
            new ModifierContainer<KnockbackModifiers, KnockbackData>();

        public override void LogicUpdate()
        {
            CheckKnockback();
        }

        public void Knockback(KnockbackData data)
        {
            var modifiedData = KnockbackModifiers.ApplyModifiers(data);
            
            Movement?.SetVelocity(modifiedData.Strength, modifiedData.Angle.normalized, modifiedData.Direction);
            Movement.CanSetVelocity = false;
            isKnockbackActive = true;
            knockbackStartTime = Time.time;
        }

        private void CheckKnockback()
        {
            if (isKnockbackActive && Movement.CurrentVelocity.y <= 0.01f &&
                (CollisionSenses.Ground || Time.time >= knockbackStartTime + maxKnockbackTime))
            {
                isKnockbackActive = false;
                Movement.CanSetVelocity = true;
            }
        }
    }
}
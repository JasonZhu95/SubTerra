namespace Project.Projectiles
{
    public class ComponentModifier<T> : ProjectileComponent<T> where T : ProjectileComponentData
    {
        protected ProjectileModifiers modifiers;
        protected bool isActive;

        public override void SetReferences()
        {
            base.SetReferences();

            isActive = TryGetComponent(out modifiers);
        }
    }
}
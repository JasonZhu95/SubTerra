namespace Project.Projectiles
{
    public class DisableDamageOnStick : ProjectileComponent<DisableDamageOnStickData>
    {
        private StickInEnvironment stickInEnvironment;
        private bool stickInEnvironmentFound;

        public override void SetReferences()
        {
            base.SetReferences();

            if (stickInEnvironmentFound = TryGetComponent(out stickInEnvironment))
            {
                stickInEnvironment.OnStick += HandleStick;
            }
        }

        private void HandleStick()
        {
            Projectile.SetCanDamage(false);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (stickInEnvironmentFound)
            {
                stickInEnvironment.OnStick -= HandleStick;
            }    
        }
    }

    public class DisableDamageOnStickData : ProjectileComponentData
    {
        public DisableDamageOnStickData()
        {
            ComponentDependencies.Add(typeof(DisableDamageOnStick));
        }
    }
}
using Project.Utilities.Notifiers;

namespace Project.Weapons
{
    public class WeaponCharge : WeaponComponent<WeaponChargeData>
    {
        private WeaponModifiers weaponModifiers;
        private readonly ChargeModifier chargeModifier = new ChargeModifier();

        private ParticleManager particleManager;
        private ParticleManager ParticleManager
        {
            get => particleManager ?? core.GetCoreComponent(ref particleManager);
        }

        private TimerNotifier timer;

        private int currentCharge;

        private void HandleInputChange(bool value)
        {
            if (value) return;

            if (currentCharge == 0)
                weapon.Anim.SetBool(WeaponBoolAnimParameters.cancel.ToString(), true);

            timer.Stop();
            chargeModifier.ModifierValue = currentCharge;
            weaponModifiers.AddModifier(chargeModifier);
        }

        private void SetCancelFalse() => weapon.Anim.SetBool(WeaponBoolAnimParameters.cancel.ToString(), false);

        public override void SetReferences()
        {
            base.SetReferences();

            weaponModifiers = GetComponent<WeaponModifiers>();
        }

        protected override void SetStartTime()
        {
            base.SetStartTime();

            var curAtkDat = data.GetAttackData(counter);

            currentCharge = curAtkDat.StartWithOne ? 1 : 0;

            timer = new TimerNotifier(curAtkDat.ChargeTime, true);
            timer.OnTimerDone += AddCharge;
        }

        private void Update()
        {
            timer?.Tick();
        }

        private void AddCharge()
        {
            currentCharge++;

            var curDat = data.GetAttackData(counter);

            if (currentCharge >= curDat.NumOfCharges)
            {
                timer.Stop();
                ParticleManager.StartParticles(curDat.MaxChargeParticlesPrefab, curDat.Offset);
            }
            else
            {
                ParticleManager.StartParticles(curDat.ChargeParticlesPrefab, curDat.Offset);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            weapon.OnInputChange += HandleInputChange;
            weapon.OnEnter += SetStartTime;
            weapon.OnExit += SetCancelFalse;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            weapon.OnInputChange -= HandleInputChange;
            weapon.OnEnter -= SetStartTime;
            weapon.OnExit -= SetCancelFalse;
        }
    }

    public class WeaponChargeData : WeaponComponentData<ChargeStruct>
    {
        public WeaponChargeData()
        {
            ComponentDependencies.Add(typeof(WeaponModifiers));
            ComponentDependencies.Add(typeof(WeaponInputHold));
            ComponentDependencies.Add(typeof(WeaponCharge));
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapons
{
    public class CancelOnInputRelease : WeaponComponent<CancelOnInputReleaseData>
    {
        private bool input;
        private bool minHoldPassed;

        private void HandleInputChange(bool value)
        {
            input = value;
            SetParams();
        }

        private void SetParams()
        {
            if (!input && !minHoldPassed) return;
            weapon.Anim.SetBool(WeaponBoolAnimParameters.hold.ToString(), input);
            weapon.Anim.SetBool(WeaponBoolAnimParameters.cancel.ToString(), input);
        }

        private void Enter()
        {
            minHoldPassed = false;
        }

        private void MinHoldPassed()
        {
            minHoldPassed = true;
            SetParams();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            weapon.OnInputChange += HandleInputChange;
            weapon.OnEnter += Enter;
            eventHandler.OnMinHold += MinHoldPassed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            weapon.OnInputChange -= HandleInputChange;
            weapon.OnEnter -= Enter;
            eventHandler.OnMinHold -= MinHoldPassed;
        }
    }

    public class CancelOnInputReleaseData : WeaponComponentData
    {
        public CancelOnInputReleaseData()
        {
            ComponentDependencies.Add(typeof(CancelOnInputRelease));
        }
    }
}

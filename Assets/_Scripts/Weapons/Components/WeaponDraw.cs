using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapons
{
    public class WeaponDraw : WeaponComponent<WeaponDrawData>
    {
        private WeaponModifiers weaponModifiers;
        private DrawModifier drawModifier = new DrawModifier();

        private void HandleInputChange(bool value)
        {
            if (!value)
            {
                var curAtkData = data.GetAttackData(counter);

                var evaluatedValue = curAtkData.curve.Evaluate(Mathf.Clamp((Time.time - attackStartTime) / curAtkData.drawTime, 0f, 1f));
                drawModifier.ModifierValue = evaluatedValue;
                weaponModifiers.AddModifier(drawModifier);
            }
        }

        public override void SetReferences()
        {
            base.SetReferences();

            weaponModifiers = GetComponent<WeaponModifiers>();
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            weapon.OnInputChange += HandleInputChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            weapon.OnInputChange -= HandleInputChange;

        }
    }

    public class WeaponDrawData : WeaponComponentData<DrawStruct>
    {
        public WeaponDrawData()
        {
            ComponentDependencies.Add(typeof(WeaponDraw));
            ComponentDependencies.Add(typeof(WeaponInputHold));
            ComponentDependencies.Add(typeof(WeaponModifiers));
        }
    }
}
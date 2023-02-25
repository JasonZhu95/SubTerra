using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Project.Weapons
{
    public class WeaponSprite : WeaponComponent<WeaponSpriteData>
    {
        private SpriteRenderer baseSpriteRenderer;
        private SpriteRenderer weaponSpriteRenderer;

        private WeaponAttackPhases currentAttackPhase = WeaponAttackPhases.Anticipation;

        private AttackPhaseSprites[] attackPhaseSprites;
        private Sprite[] sprites;

        private WeaponSprites currentAttackData;

        private int currentWeaponSpriteIndex = 0;

        public override void SetReferences()
        {
            base.SetReferences();

            baseSpriteRenderer = weapon.BaseGO.GetComponent<SpriteRenderer>();
            weaponSpriteRenderer = transform.Find("WeaponSprite").GetComponent<SpriteRenderer>();
        }

        protected override void SetCurrentAttackData()
        {
            base.SetCurrentAttackData();
            currentAttackData = data.GetAttackData(counter);
        }

        private void SetPhase(WeaponAttackPhases phase)
        {
            currentAttackPhase = phase;
            currentWeaponSpriteIndex = 0;

            attackPhaseSprites = currentAttackData.AttackPhases;
            sprites = attackPhaseSprites.Where(attackPhaseSprites => attackPhaseSprites.Phase == currentAttackPhase).FirstOrDefault().Sprites;
        }

        private void OnBaseSpriteChange(SpriteRenderer renderer)
        {
            weaponSpriteRenderer.sprite = sprites[currentWeaponSpriteIndex];
            currentWeaponSpriteIndex++;

            if (currentWeaponSpriteIndex >= sprites.Length) currentWeaponSpriteIndex = 0;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            eventHandler.OnEnterAttackPhase += SetPhase;
            baseSpriteRenderer.RegisterSpriteChangeCallback(OnBaseSpriteChange);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            baseSpriteRenderer.sprite = null;
            weaponSpriteRenderer.sprite = null;

            eventHandler.OnEnterAttackPhase -= SetPhase;
            baseSpriteRenderer.UnregisterSpriteChangeCallback(OnBaseSpriteChange);
        }
    }

    public class WeaponSpriteData : WeaponComponentData<WeaponSprites>
    {
        public WeaponSpriteData()
        {
            ComponentDependencies.Add(typeof(WeaponSprite));
        }

        public override void OnValidate()
        {
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].AttackPhases.Length; j++)
                {
                    data[i].AttackPhases[j].Name = data[i].AttackPhases[j].Phase.ToString();
                }
            }
        }
    }
}

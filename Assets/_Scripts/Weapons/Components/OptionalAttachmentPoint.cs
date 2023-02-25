using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Weapons
{
    public class OptionalAttachmentPoint : WeaponComponent<OptionalAttachmentPointData>
    {
        private SpriteRenderer optionalSpriteRenderer;

        public override void SetReferences()
        {
            base.SetReferences();

            optionalSpriteRenderer = transform.Find("Base/OptionalSprite")?.GetComponent<SpriteRenderer>();
            optionalSpriteRenderer.enabled = false;
        }

        private void Enter()
        {
            var currentAttackData = data.GetAttackData(counter);

            if (currentAttackData.UseSprite)
            {
                optionalSpriteRenderer.sprite = currentAttackData.Sprite;
            }
        }

        private void Exit()
        {
            optionalSpriteRenderer.sprite = null;
        }

        private void EnableSpite() => optionalSpriteRenderer.enabled = true;
        private void DisableSpite() => optionalSpriteRenderer.enabled = false;

        protected override void OnEnable()
        {
            base.OnEnable();

            eventHandler.OnEnableOptionalSprite += EnableSpite;
            eventHandler.OnDisableOptionalSprite += DisableSpite;
            weapon.OnEnter += Enter;
            weapon.OnExit += Exit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            eventHandler.OnEnableOptionalSprite -= EnableSpite;
            eventHandler.OnDisableOptionalSprite -= DisableSpite;
            weapon.OnEnter -= Enter;
            weapon.OnExit -= Exit;
        }
    }

    public class OptionalAttachmentPointData : WeaponComponentData<OptionalAttachmentPointStruct>
    {
        public OptionalAttachmentPointData() => ComponentDependencies.Add(typeof(OptionalAttachmentPoint));
    }
}
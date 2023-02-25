using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Project.Weapons
{
    public class WeaponAnimationEventHandler : MonoBehaviour
    {
        public event Action OnFinish;
        public event Action OnStartMovement;
        public event Action OnStopMovement;
        public event Action OnAttackAction;
        public event Action OnEnableFlip;
        public event Action OnDisableFlip;
        public event Action OnEnableOptionalSprite;
        public event Action OnDisableOptionalSprite;
        public event Action OnMinHold;
        public event Action OnEnableInterrupt;

        public event Action<WeaponAttackPhases> OnEnterAttackPhase;

        private void EnterAttackPhaseTrigger(WeaponAttackPhases phase)
        {
            OnEnterAttackPhase?.Invoke(phase);
        }

        private void DisableFlipTrigger()
        {
            OnDisableFlip?.Invoke();
        }

        private void EnableFlipTrigger()
        {
            OnEnableFlip?.Invoke();
        }

        private void AttackActionTrigger()
        {
            OnAttackAction?.Invoke();
        }

        private void AnimationFinishTrigger()
        {
            OnFinish?.Invoke();
        }

        private void StartMovementTrigger()
        {
            OnStartMovement?.Invoke();
        }

        private void StopMovementTrigger()
        {
            OnStopMovement?.Invoke();
        }

        private void EnableOptionalSpriteTrigger()
        {
            OnEnableOptionalSprite?.Invoke();
        }

        private void DisableOptionalSpriteTrigger()
        {
            OnDisableOptionalSprite?.Invoke();
        }

        private void MinHoldTrigger()
        {
            OnMinHold?.Invoke();
        }

        private void EnableInterruptTrigger()
        {
            OnEnableInterrupt?.Invoke();
        }
    }
}

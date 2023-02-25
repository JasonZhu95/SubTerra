using System;
using Project.Combat.Interfaces;
using UnityEngine;

namespace Project.CoreComponents
{
    public class ParryComponent : CoreComponent, IParryable
    {
        private Collider2D parryCollider;

        public event Action OnParried;
        
        public void Parry(ParryData data)
        {
            Debug.Log("Parry Collider Detected");
            OnParried?.Invoke();
        }

        public GameObject GetParent()
        {
            return core.Parent;
        }

        protected override void Awake()
        {
            base.Awake();
            parryCollider = GetComponent<Collider2D>();
            parryCollider.enabled = false;
        }

        // Set collider active from animation events when an attack is parryable
        public void SetParryColliderActive(bool value) => parryCollider.enabled = value;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    protected Weapon weapon;
    protected WeaponAnimationEventHandler eventHandler;
    protected Core core;
    protected int counter;
    protected float attackStartTime;

    public virtual void SetReferences()
    {
        counter = 0;

        weapon = GetComponent<Weapon>();
        core = weapon.Core;

        eventHandler = GetComponentInChildren<WeaponAnimationEventHandler>();
    }

    protected virtual void SetCounter(int i) => counter = i;
    protected virtual void SetStartTime() => attackStartTime = Time.time;

    protected virtual void OnEnable()
    {
        weapon.OnCounterChange += SetCounter;
        weapon.OnEnter += SetStartTime;
    }

    protected virtual void OnDisable()
    {
        weapon.OnCounterChange -= SetCounter;
        weapon.OnEnter -= SetStartTime;
    }
}

public class WeaponComponent<T> : WeaponComponent where T : WeaponComponentData
{
    protected T data;

    protected virtual void SetCurrentAttackData() { }

    public override void SetReferences()
    {
        base.SetReferences();
        data = weapon.WeaponData.GetComponentData<T>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        weapon.OnEnter += SetCurrentAttackData;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        weapon.OnEnter -= SetCurrentAttackData;
    }
}

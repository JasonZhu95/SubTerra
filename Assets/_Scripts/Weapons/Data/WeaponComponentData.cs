using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class WeaponComponentData
{
    private List<Type> componentDependencies = new List<Type>();

    public List<Type> ComponentDependencies { get => componentDependencies; protected set => componentDependencies = value; }

    public WeaponComponentData()
    {
        ComponentName = this.GetType().Name;
    }

    [field: SerializeField, HideInInspector] public string ComponentName { get; private set; }

    public virtual void SetNumberOfAttacks(int value) { }

    public virtual void OnValidate() { }
}

[System.Serializable]
public class WeaponComponentData<T> : WeaponComponentData where T : INameable
{
    [field: SerializeField] public bool repeatData;

    [SerializeField] protected T[] data;

    public T[] GetAllData() => data;

    public T GetAttackData(int i)
    {
        if (repeatData) i = 0;
        return data[i];
    }

    public override void SetNumberOfAttacks(int value)
    {
        if (repeatData) value = 1;

        Array.Resize(ref data, value);

        for (int i = 0; i < data.Length; i++)
        {
            data[i].SetName("Attack " + (i + 1));
        }
    }
}

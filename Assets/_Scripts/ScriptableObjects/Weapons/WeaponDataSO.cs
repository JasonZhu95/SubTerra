using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data/Basic Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [field: SerializeField] public int NumberOfAttacks { get; private set; }
    
    [field: SerializeReference] public List<ComponentData> ComponentDataList { get; private set; }

    public T GetData<T>()
    {
        return ComponentDataList.OfType<T>().FirstOrDefault();
    }

    public void AddData(ComponentData data)
    {
        if (ComponentDataList.FirstOrDefault(t => t.GetType() == data.GetType()) != null)
        {
            return;
        }

        ComponentDataList.Add(data);
    }

    public List<Type> GetAllDependencies()
    {
        return ComponentDataList.Select(component => component.ComponentDependency).ToList();
    }
}

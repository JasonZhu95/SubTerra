using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using Project.Weapons;

[CustomEditor(typeof(WeaponDataSO))]
public class WeaponDataEditor : Editor
{
    public List<Type> dataCompTypes = new List<Type>();

    public override void OnInspectorGUI()
    {
        WeaponDataSO data = target as WeaponDataSO;

        foreach (Type T in dataCompTypes)
        {
            if (GUILayout.Button(T.Name))
            {
                var dataType = Activator.CreateInstance(T);

                if (dataType.GetType().IsSubclassOf(typeof(WeaponComponentData)))
                {
                    data.AddDataComponent(dataType as WeaponComponentData);
                }

                EditorUtility.SetDirty(this);
            }
        }

        if (GUILayout.Button("Set Weapon Data Attack Number"))
        {
            foreach (WeaponComponentData dataComp in data.ComponentDatas)
            {
                dataComp.SetNumberOfAttacks(data.NumberOfAttacks);
            }
        }

        DrawDefaultInspector();
    }

    [ExecuteInEditMode]
    private void OnEnable()
    {
        var returned = GetAllWeaponDataComponents();
        dataCompTypes.Clear();

        returned.ToList().ForEach(item =>
        {
            dataCompTypes.Add(item.GetType());
        });

    }

    private IEnumerable<WeaponComponentData> GetAllWeaponDataComponents()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => type.IsSubclassOf(typeof(WeaponComponentData)) && type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters)
        .Select(type => Activator.CreateInstance(type) as WeaponComponentData);
    }
}
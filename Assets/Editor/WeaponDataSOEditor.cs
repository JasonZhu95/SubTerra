using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Callbacks;
using System.Linq;

[CustomEditor(typeof(WeaponDataSO))]
public class WeaponDataSOEditor : Editor
{
    private static List<Type> dataCompTypes = new List<Type>();

    private WeaponDataSO dataSO;

    private bool showForceUpdateButtons;
    private bool showAddComponentButtons;

    private void OnEnable()
    {
        dataSO = target as WeaponDataSO;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Set Number Of Attacks"))
        {
            foreach (var item in dataSO.ComponentDataList)
            {
                item.InitializeAttackData(dataSO.NumberOfAttacks);
            }
        }

        showAddComponentButtons = EditorGUILayout.Foldout(showAddComponentButtons, "Add Components");
        showForceUpdateButtons = EditorGUILayout.Foldout(showForceUpdateButtons, "Force Update");

        if (showAddComponentButtons)
        {
            foreach (var dataCompTypes in dataCompTypes)
            {
                if (GUILayout.Button(dataCompTypes.Name))
                {
                    var comp = Activator.CreateInstance(dataCompTypes) as ComponentData;

                    if (comp == null)
                        return;

                    comp.InitializeAttackData(dataSO.NumberOfAttacks);

                    dataSO.AddData(comp);
                }
            }
        }

        if (showForceUpdateButtons)
        {
            if(GUILayout.Button("Force Update Component Names"))
            {
                foreach (var item in dataSO.ComponentDataList)
                {
                    item.SetComponentName();
                }
            }

            if (GUILayout.Button("Force Update Attack Names"))
            {
                foreach (var item in dataSO.ComponentDataList)
                {
                    item.SetAttackDataNames();
                }
            }
        }
    }

    [DidReloadScripts]
    private static void OnRecompile()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var types = assemblies.SelectMany(assembly => assembly.GetTypes());
        var filteredTypes = types.Where(
            type => type.IsSubclassOf(typeof(ComponentData)) && !type.ContainsGenericParameters && type.IsClass
        );
        dataCompTypes = filteredTypes.ToList();

    }
}

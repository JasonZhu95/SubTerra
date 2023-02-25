using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Project.Projectiles;

[CustomEditor(typeof(ProjectileDataSO))]
public class ProjectileDataEditor : Editor
{
    public List<Type> dataCompTypes = new List<Type>();

    public override void OnInspectorGUI()
    {
        ProjectileDataSO data = target as ProjectileDataSO;

        foreach (Type T in dataCompTypes)
        {
            if (GUILayout.Button(T.Name))
            {
                var dataType = Activator.CreateInstance(T);

                if (dataType.GetType().IsSubclassOf(typeof(ProjectileComponentData)))
                {
                    data.AddDataComponent(dataType as ProjectileComponentData);
                }
            }
        }
        DrawDefaultInspector();
    }

    [ExecuteInEditMode]
    private void OnEnable()
    {
        var returned = GetAllProjectileComponentDatas();
        dataCompTypes.Clear();

        returned.ToList().ForEach(item =>
        {
            dataCompTypes.Add(item.GetType());
        });
    }

    private IEnumerable<ProjectileComponentData> GetAllProjectileComponentDatas()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => type.IsSubclassOf(typeof(ProjectileComponentData)))
        .Select(type => Activator.CreateInstance(type) as ProjectileComponentData);
    }
}

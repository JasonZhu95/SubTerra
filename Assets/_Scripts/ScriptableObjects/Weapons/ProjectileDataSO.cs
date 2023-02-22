using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileDataSO : ScriptableObject
{
    [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }
    [field: SerializeField] public LayerMask interactableLayers { get; private set; }
    [field: SerializeReference] public List<ProjectileComponentData> ComponentDatas { get; private set; }

    public T GetComponentData<T>() where T : ProjectileComponentData
    {
        return ComponentDatas.OfType<T>().FirstOrDefault();
    }

    public List<Type> GetAllDependencies()
    {
        List<Type> dependencies = new List<Type>();

        foreach (var item in ComponentDatas)
        {
            foreach (var dependency in item.ComponentDependencies)
            {
                if (dependencies.FirstOrDefault(e => e.GetType() == dependency) == null)
                {
                    dependencies.Add(dependency);
                }
            }
        }

        return dependencies;
    }

#if UNITY_EDITOR
    public void AddDataComponent<T>(T data) where T : ProjectileComponentData
    {
        if (ComponentDatas.Where(item => item.GetType() == data.GetType()).FirstOrDefault() != null) return;
        ComponentDatas.Add(data);
    }
#endif
}

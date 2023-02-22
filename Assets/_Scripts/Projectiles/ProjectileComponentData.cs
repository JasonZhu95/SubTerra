using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class ProjectileComponentData
{
    private List<Type> componentDependencies = new List<Type>();
    public List<Type> ComponentDependencies { get => componentDependencies; protected set => componentDependencies = value; }

    public ProjectileComponentData()
    {
        this.name = this.GetType().Name;
    }

    [SerializeField, HideInInspector]
    protected string name = "Test";
}

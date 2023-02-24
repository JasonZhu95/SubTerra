using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Core : MonoBehaviour
{
    public Transform EntityTransform { get; private set; }
    private readonly List<CoreComponent> CoreComponents = new List<CoreComponent>();

    [field: SerializeField] public GameObject Parent { get; private set; }

    private void Awake()
    {
        EntityTransform = transform.parent.transform;

        var comps = GetComponentsInChildren<CoreComponent>();

        foreach (var component in comps)
        {
            AddComponent(component);
        }

        foreach (var component in CoreComponents)
        {
            component.Init(this);
        }
    }

    public void LogicUpdate()
    {
        foreach (CoreComponent component in CoreComponents)
        {
            component.LogicUpdate();
        }
    }

    public void AddComponent(CoreComponent component)
    {
        if (!CoreComponents.Contains(component))
        {
            CoreComponents.Add(component);
        }
    }

    // FUNCTION: Accesses the List of Core Components and returns the type of component.
    public T GetCoreComponent<T>() where T : CoreComponent
    {
        var comp = CoreComponents
            .OfType<T>()
            .FirstOrDefault();

        if (comp == null)
        {
            Debug.LogWarning($"{typeof(T)} not found on {transform.parent.name}");
        }

        return comp;
    }

    public T GetCoreComponent<T>(ref T value) where T : CoreComponent
    {
        value = GetCoreComponent<T>();
        return value;
    }
}

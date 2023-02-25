using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Core : MonoBehaviour
{
    public Transform EntityTransform { get; private set; }
    public readonly List<CoreComponent> CoreComponents = new List<CoreComponent>();

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

    public void AddComponent(CoreComponent component)
    {
        if (!CoreComponents.Contains(component))
        {
            Debug.Log($"{component} was added");
            CoreComponents.Add(component);
        }
    }
}

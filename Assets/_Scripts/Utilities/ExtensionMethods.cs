using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component)
    {
        component = gameObject.GetComponentInChildren<T>();
        return component != null;
    }

    public static bool TryGetComponentsInChildren<T>(this GameObject gameObject, out T[] components)
    {
        components = gameObject.GetComponentsInChildren<T>();
        return components.Length != 0;
    }
}

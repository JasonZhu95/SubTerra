using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameObjectUtilities
{
    public static List<T> AddDependenciesToGO<T>(this GameObject gameObject, List<Type> dependencies) where T : Component
    {
        List<T> currentComps = gameObject.GetComponents<T>().ToList();
        List<T> addedComps = new List<T>();

        foreach (Type dependency in dependencies)
        {
            if (addedComps.FirstOrDefault(item => item.GetType() == dependency) != null) continue;

            var comp = currentComps.FirstOrDefault(item => item.GetType() == dependency);

            if (comp == null)
            {
                comp = (T)gameObject.AddComponent(dependency);
            }
            else
            {
                currentComps.Remove(comp);
            }

            addedComps.Add(comp);
        }

        foreach (T comp in currentComps)
        {
#if UNITY_EDITOR
            UnityEngine.Object.DestroyImmediate(comp);
#else
            UnityEngine.Object.Destroy(comp);
#endif
        }

        return addedComps;
    }
}

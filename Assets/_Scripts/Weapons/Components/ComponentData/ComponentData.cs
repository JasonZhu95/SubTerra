using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ComponentData
{

}

[Serializable]
public class ComponentData<T> : ComponentData where T : AttackData
{
    [field: SerializeField] public T[] AttackData { get; private set; }
}

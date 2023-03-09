using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerable
{
    void TriggerObject(TriggerableData data);
}

public struct TriggerableData
{
    public GameObject Source;
    public bool isSet;

    public void SetData(GameObject source, bool setBool)
    {
        Source = source;
        isSet = setBool;
    }
}
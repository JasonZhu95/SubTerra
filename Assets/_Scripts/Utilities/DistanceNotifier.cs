using System;
using UnityEngine;

public class DistanceNotifier
{
    public event Action OnTarget;

    private Vector3 target;
    private float distance;
    private bool inside;

    private bool canTriggerEvent;

    public DistanceNotifier(Vector3 target, float distance, bool inside, bool triggerOnce)
    {
        this.target = target;
        this.distance = distance;
        this.inside = inside;

        canTriggerEvent = true;

        if (triggerOnce)
        {
            OnTarget += () => canTriggerEvent = false;
        }
    }

    public void Tick(Vector3 pos)
    {
        if (inside)
        {
            CheckInside(pos);
        }
        else
        {
            CheckOutside(pos);
        }
    }

    private void CheckInside(Vector3 pos)
    {
        if (Vector3.Distance(pos, target) <= distance && canTriggerEvent)
        {
            OnTarget?.Invoke();
        }
    }

    private void CheckOutside(Vector3 pos)
    {
        if (Vector3.Distance(pos, target) >= distance && canTriggerEvent)
        {
            OnTarget?.Invoke();
        }
    }
}
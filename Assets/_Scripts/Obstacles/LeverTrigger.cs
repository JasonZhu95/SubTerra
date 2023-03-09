using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : MonoBehaviour, ITriggerable
{
    [SerializeField] private DoorAnimated door;

    public void TriggerObject(TriggerableData data)
    {
        if (data.isSet)
        {
            door.OpenDoor();
        }
        else
        {
            door.CloseDoor();
        }
    }
}

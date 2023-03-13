using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimated : MonoBehaviour, IDataPersistence
{
    private Animator doorAnim;
    private bool doorOpen;

    private void Awake()
    {
        doorAnim = transform.parent.GetComponent<Animator>();
        doorOpen = false;
    }

    public void OpenDoor()
    {
        doorOpen = true;
        doorAnim.SetBool("open", doorOpen);
    }

    public void CloseDoor()
    {
        doorOpen = false;
        doorAnim.SetBool("open", doorOpen);
    }

    public void LoadData(GameData data)
    {
        doorOpen = data.doorStatus;
        if (doorOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    public void SaveData(ref GameData data)
    {
        data.doorStatus = doorOpen;
    }
}

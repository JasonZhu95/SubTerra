using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimated : MonoBehaviour
{
    private Animator doorAnim;

    private void Awake()
    {
        doorAnim = transform.parent.GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        doorAnim.SetBool("open", true);
    }

    public void CloseDoor()
    {
        doorAnim.SetBool("open", false);
    }
}

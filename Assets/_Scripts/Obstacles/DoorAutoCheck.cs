using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAutoCheck : MonoBehaviour
{
    [SerializeField]
    private DoorAnimated door;
    private bool isOpen;

    private void Start()
    {
        door.OpenDoor();
        isOpen = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                door.CloseDoor();
                isOpen = false;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAutoCheck : MonoBehaviour
{
    [SerializeField]
    private DoorAnimated door;

    private void Start()
    {
        door.OpenDoor();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (door.doorOpen)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                door.CloseDoor();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!door.doorOpen)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                door.OpenDoor();
            }
        }
    }
}

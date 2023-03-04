using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Inventory.Data;

public class PlayerObstacleCollision : MonoBehaviour
{
    [SerializeField]
    private InventorySO inventoryData;

    [SerializeField]
    private PlayerData playerData;

    private GameObject player;

    private Rigidbody2D playerRB;

    public bool trampolineDetected;

    private Vector2 currentVelocity;
    private Vector2 workspace;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            int reminder = inventoryData.AddItem(item.InventoryItem, item.Quantity);
            if (reminder == 0)
                item.DestroyItem();
            else
                item.Quantity = reminder;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trampoline"))
        {
            trampolineDetected = true;
            workspace.Set(currentVelocity.x, playerData.trampolineVelocity);
            playerRB.velocity = workspace;
            currentVelocity = workspace;
        }

        if (collision.gameObject.CompareTag("Spike"))
        {
            Destroy(player);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trampoline"))
        {
            trampolineDetected = false;
        }
    }
}

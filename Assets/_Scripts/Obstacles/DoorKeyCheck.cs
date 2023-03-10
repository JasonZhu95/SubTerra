using UnityEngine;
using Project.Combat.Interfaces;
using Project.Inventory;
using Project.Inventory.Data;

public class DoorKeyCheck : MonoBehaviour, IInteractable
{
    private bool playerInRange;

    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [SerializeField] private DoorAnimated door;

    private GameObject player;
    private PlayerInputHandler inputHandler;
    private bool deactivateUpdate;
    private InventorySO inventoryData;
    private bool playerHasKey;

    private void Start()
    {
        playerHasKey = false;
        player = GameObject.FindWithTag("Player");
        inputHandler = player.GetComponent<PlayerInputHandler>();
        visualCue.SetActive(false);
        deactivateUpdate = false;
        inventoryData = player.GetComponent<InventoryController>().inventoryData;
    }

    private void Update()
    {
        if (!deactivateUpdate)
        {
            if (playerInRange)
            {
                visualCue.SetActive(true);
                if (inputHandler.InteractPressed)
                {
                    inputHandler.InteractPressed = false;
                    CheckIfHasKey();
                }
            }
            else
            {
                visualCue.SetActive(false);
            }
        }
    }

    private void CheckIfHasKey()
    {
        playerHasKey = inventoryData.SearchInventoryAndRemove("Key");
        if (playerHasKey)
        {
            door.OpenDoor();
            visualCue.SetActive(false);
            deactivateUpdate = true;
        }
    }

    public object GetInteractionContext()
    {
        return null;
    }

    public void SetContext(object obj)
    {

    }

    public void EnableInteraction()
    {
        playerInRange = true;
    }

    public void DisableInteraction()
    {
        playerInRange = false;
    }

    public Vector3 GetPosition() => transform.position;
}

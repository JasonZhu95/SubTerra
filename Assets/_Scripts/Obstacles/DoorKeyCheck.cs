using UnityEngine;
using Project.Interfaces;
using Project.Inventory;
using Project.Inventory.Data;
using Project.UI;

public class DoorKeyCheck : MonoBehaviour, IInteractable
{
    private bool playerInRange;

    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    [SerializeField] private DoorAnimated door;

    [Header("First Door Checks")]
    [SerializeField] private bool firstDoor;
    [SerializeField] private TextAsset inkJSON;

    private GameObject player;
    private CollisionSenses playerCollision;
    private PlayerInputHandler inputHandler;
    private bool deactivateUpdate;
    private InventorySO inventoryData;
    private bool playerHasKey;
    private DialogueManager dialogueManagerReference;

    private void Start()
    {
        playerHasKey = false;
        player = GameObject.FindWithTag("Player");
        playerCollision = player.GetComponent<Player>().Core.transform.GetChild(1).GetComponent<CollisionSenses>();
        inputHandler = player.GetComponent<PlayerInputHandler>();
        visualCue.SetActive(false);
        deactivateUpdate = false;
        inventoryData = player.GetComponent<InventoryController>().inventoryData;
        dialogueManagerReference = GameObject.FindWithTag("DialogueContainer").GetComponent<DialogueManager>();
    }

    private void Update()
    {
        if (!deactivateUpdate)
        {
            if (playerInRange && playerCollision.Ground)
            {
                visualCue.SetActive(true);
                if (inputHandler.InteractPressed)
                {
                    inputHandler.InteractPressed = false;
                    if (firstDoor)
                    {
                        firstDoor = false;
                        dialogueManagerReference.EnterDialogueMode(inkJSON);
                    }
                    else
                    {
                        CheckIfHasKey();
                    }
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

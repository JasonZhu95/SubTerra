using UnityEngine;
using Project.Interfaces;
using Project.EventChannels;
using Project.Managers;
using Project.Inventory;

public class ShopTrigger : MonoBehaviour, IInteractable
{
    private bool playerInRange;

    [Header("Visual Cue")]
    [SerializeField] private GameObject visualCue;

    private GameObject player;
    private PlayerInputHandler inputHandler;
    private IBuyItem customer;

    [SerializeField]
    private UI_Shop shopUI;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        inputHandler = player.GetComponent<PlayerInputHandler>();
        visualCue.SetActive(false);
        customer = player.GetComponent<InventoryController>().GetComponent<IBuyItem>();
    }

    private void Update()
    {
        if (playerInRange)
        {
            visualCue.SetActive(true);
            if (inputHandler.InteractShopPressed)
            {
                if (!shopUI.isActiveAndEnabled)
                {
                    shopUI.Show(customer);
                    inputHandler.SwitchToActionMap("UI");
                }
            }
            else if(shopUI.isActiveAndEnabled)
            {
                shopUI.Hide();
                inputHandler.SwitchToActionMap("Gameplay");
            }
        }
        else
        {
            visualCue.SetActive(false);
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

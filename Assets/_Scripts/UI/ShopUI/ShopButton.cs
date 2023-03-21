using Project.Inventory.Data;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private GameObject shopPanelToOpen;
    [SerializeField]
    private TextMeshProUGUI confirmTitleText;
    [SerializeField]
    private TextMeshProUGUI confirmCoinText;
    [SerializeField]
    private Image confirmItemImage;

    public ShopButtonController shopButtonController;
    public Animator animator;
    public int thisIndex;

    private PlayerInputHandler inputHandler;

    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private ShopConfirmButton shopConfirmButton;

    private PlayerInventory playerInventory;

    private bool canAfford;
    private Image borderSpriteRenderer;
    private Color disablePurchaseColor;

    public ItemSO ItemData { get; set; }

    private void Awake()
    {
        inputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
        playerInventory = GameObject.FindWithTag("Player").transform.GetChild(0).GetComponentInChildren<PlayerInventory>();
        borderSpriteRenderer = transform.GetChild(0).GetComponent<Image>();
        disablePurchaseColor = borderSpriteRenderer.color;
    }

    private void Update()
    {
        // Check if player can afford the item
        if (ItemData.ItemCost > playerInventory.CoinsHeld)
        {
            disablePurchaseColor.a = 0.6f;
            borderSpriteRenderer.color = disablePurchaseColor;
            canAfford = false;
        }
        else
        {
            disablePurchaseColor.a = 1.0f;
            borderSpriteRenderer.color = disablePurchaseColor;
            canAfford = true;
        }

        // Check if this is the selected button
        if (shopButtonController.index == thisIndex)
        {
            if (titleText != null && descriptionText != null)
            {
                titleText.text = ItemData.Name;
                descriptionText.text = ItemData.Description;
            }
            animator.SetBool("selected", true);
            if (inputHandler.MainActionUIInput && !canAfford)
            {
                inputHandler.MainActionUIInput = false;
            }
            if (inputHandler.MainActionUIInput && canAfford)
            {
                animator.SetBool("pressed", true);
                if (shopPanelToOpen != null)
                {
                    inputHandler.MainActionUIInput = false;
                    confirmTitleText.text = ItemData.Name;
                    confirmCoinText.text = ItemData.ItemCost.ToString();
                    confirmItemImage.sprite = ItemData.ItemImage;
                    shopConfirmButton.ItemData = ItemData;

                    shopButtonController.gameObject.SetActive(false);
                    shopPanelToOpen.SetActive(true);
                }
            }
            else if (animator.GetBool("pressed"))
            {
                animator.SetBool("pressed", false);
            }
        }
        else
        {
            animator.SetBool("selected", false);
        }
    }
}

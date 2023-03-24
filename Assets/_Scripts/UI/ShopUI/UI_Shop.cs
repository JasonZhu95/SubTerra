using Project.Inventory.Data;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Project.Interfaces;

public class UI_Shop : MonoBehaviour
{
    private ShopButton shopButton;
    private ShopButtonController shopButtonController;
    private Transform shopItemTemplate;
    public Transform itemContainer;
    private IBuyItem customer;
    private int maxIndexAmount;
    
    [SerializeField]
    private ItemDatabase itemDatabase;

    private void Awake()
    {
        shopItemTemplate = itemContainer.Find("ItemTemplate");
        shopButtonController = shopItemTemplate.parent.GetComponent<ShopButtonController>();
        shopItemTemplate.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        CreateItemButton(itemDatabase.GetItem(0), 0);
        CreateItemButton(itemDatabase.GetItem(1), 1);
        CreateItemButton(itemDatabase.GetItem(2), 2);
        CreateItemButton(itemDatabase.GetItem(3), 3);
        CreateItemButton(itemDatabase.GetItem(5), 4);
    }

    private void CreateItemButton(ItemSO itemData, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, itemContainer);
        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();
        shopButton = shopItemTransform.GetComponent<ShopButton>();
        shopButton.ItemData = itemData;
        shopButton.thisIndex = positionIndex;

        float shopItemHeight = 100f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("PriceText").GetComponent<TextMeshProUGUI>().SetText(itemData.ItemCost.ToString());
        shopItemTransform.Find("ItemImage").GetComponent<Image>().sprite = itemData.ItemImage;

        if (positionIndex > maxIndexAmount)
        {
            maxIndexAmount = positionIndex;
            shopButtonController.maxIndex = maxIndexAmount;
        }
    }

    public void OnItemTemplateClick(ItemSO itemData)
    {
        TryBuyItem(itemData);
    }

    private void TryBuyItem(ItemSO itemData)
    {
        if (customer.TrySpendCurrency(itemData.ItemCost))
        {
            // Can afford the item
            customer.BoughtItem(itemData);
        }
    }

    public void Show(IBuyItem customer)
    {
        this.customer = customer;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

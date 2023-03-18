using Project.Inventory.Data;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Project.Interfaces;

public class UI_Shop : MonoBehaviour
{
    private Transform shopItemTemplate;
    public Transform container;
    private IBuyItem customer;
    
    [SerializeField]
    private ItemDatabase itemDatabase;

    private void Awake()
    {
        shopItemTemplate = container.Find("ShopItemTemplate");
        shopItemTemplate.gameObject.SetActive(false);
        container.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void Start()
    {
        //CreateItemButton(itemDatabase.GetItem(0), 0);
        //CreateItemButton(itemDatabase.GetItem(1), 1);
        //CreateItemButton(itemDatabase.GetItem(2), 2);
        //CreateItemButton(itemDatabase.GetItem(3), 3);
        //CreateItemButton(itemDatabase.GetItem(5), 4);
    }

    private void CreateItemButton(ItemSO itemData, int positionIndex)
    {
        Transform shopItemTransform = Instantiate(shopItemTemplate, container);
        shopItemTransform.gameObject.SetActive(true);
        RectTransform shopItemRectTransform = shopItemTransform.GetComponent<RectTransform>();

        float shopItemHeight = 110f;
        shopItemRectTransform.anchoredPosition = new Vector2(0, -shopItemHeight * positionIndex);

        shopItemTransform.Find("NameText").GetComponent<TextMeshProUGUI>().SetText(itemData.Name);
        shopItemTransform.Find("PriceText").GetComponent<TextMeshProUGUI>().SetText(itemData.ItemCost.ToString());
        shopItemTransform.Find("ItemImage").GetComponent<Image>().sprite = itemData.ItemImage;

        Button btn = shopItemTransform.GetComponent<Button>();
        btn.onClick.AddListener(() => OnItemTemplateClick(itemData));
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

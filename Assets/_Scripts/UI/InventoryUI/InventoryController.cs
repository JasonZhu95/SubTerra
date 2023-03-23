using UnityEngine;
using Project.EventChannels;
using Project.Managers;
using Project.Inventory.UI;
using Project.Inventory.Data;
using System.Collections.Generic;
using System.Text;
using Project.Interfaces;
using System.Collections;

namespace Project.Inventory
{
    public class InventoryController : MonoBehaviour, IDataPersistence, IBuyItem
    {
        [SerializeField] private UIInventoryPage inventoryUI;

        [SerializeField] public InventorySO inventoryData;

        [SerializeField]
        private AudioClip dropClip;

        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private ItemActionPanel itemActionPanelObject;

        public PlayerInputHandler InputHandler { get; private set; }

        [SerializeField]
        private ItemDatabase database;

        public List<InventoryItem> initialItems = new List<InventoryItem>();
        public List<InventoryItem> itemsToAdd = new List<InventoryItem>();

        private PlayerInventory playerInventory;
        private int menuXInput;
        private int menuYInput;
        private float lastTime;

        private void Awake()
        {
            InputHandler = GetComponent<PlayerInputHandler>();
            playerInventory = transform.GetChild(0).GetComponentInChildren<PlayerInventory>();
        }

        private void Start()
        {
            PrepareUI();
            PrepareInventoryData();
            lastTime = Time.unscaledTime;
        }

        private void Update()
        {

            // Check if Player Opens the inventory
            if (InputHandler.InventoryPressed)
            {
                if (inventoryUI.isActiveAndEnabled == false)
                {
                    inventoryUI.Show();
                    foreach (var item in inventoryData.GetCurrentInventoryState())
                    {
                        inventoryUI.UpdateData(item.Key,
                            item.Value.item.ItemImage,
                            item.Value.quantity);
                    }
                    InputHandler.SwitchToActionMap("UI");

                }
            }
            else if (!InputHandler.InventoryPressed && inventoryUI.isActiveAndEnabled == true)
            {
                inventoryUI.Hide();
                InputHandler.SwitchToActionMap("Gameplay");
            }

            // Do Menu Actions
            if (inventoryUI.isActiveAndEnabled)
            {
                menuXInput = InputHandler.NormMenuInputX;
                menuYInput = InputHandler.NormMenuInputY;

                if (menuYInput != 0 && (Time.unscaledTime - lastTime > 0.5f))
                {
                    lastTime = Time.unscaledTime;
                    Debug.Log("Pressed1");
                    if (menuYInput == 1)
                    {
                        if ((inventoryUI.currentlySelectedIndex - 6) < 0)
                        {
                            inventoryUI.SelectItemIndex((inventoryUI.currentlySelectedIndex - 6) + 18);
                        }
                        else
                        {
                            inventoryUI.SelectItemIndex(inventoryUI.currentlySelectedIndex - 6);
                        }
                    }
                    if (menuYInput == -1)
                    {
                        if ((inventoryUI.currentlySelectedIndex + 6) >= 18)
                        {
                            inventoryUI.SelectItemIndex((inventoryUI.currentlySelectedIndex + 6) % 18);
                            Debug.Log((inventoryUI.currentlySelectedIndex + 6) % 18);
                        }
                        else
                        {
                            inventoryUI.SelectItemIndex(inventoryUI.currentlySelectedIndex + 6);
                        }
                    }
                }
                if (menuXInput != 0 && (Time.unscaledTime - lastTime > 0.1f))
                {
                    Debug.Log("Pressed2");
                    lastTime = Time.unscaledTime;
                    if (menuXInput == 1)
                    {
                        if (inventoryUI.currentlySelectedIndex == 17)
                        {
                            inventoryUI.SelectItemIndex(0);
                        }
                        else
                        {
                            inventoryUI.SelectItemIndex(inventoryUI.currentlySelectedIndex + 1);
                        }
                    }
                    if (menuXInput == -1)
                    {
                        if (inventoryUI.currentlySelectedIndex == 0)
                        {
                            inventoryUI.SelectItemIndex(17);
                        }
                        else
                        {
                            inventoryUI.SelectItemIndex(inventoryUI.currentlySelectedIndex - 1);
                        }
                    }
                }
            }
        }

        // On Gamestart load any items in save file or any starting initial items.
        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
            foreach (InventoryItem item in itemsToAdd)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
            
        }

        // Update the UI when item data is changed
        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage,
                    item.Value.quantity);
            }
        }

        // Add Events onto the UI when user interacts with the inventory.
        private void PrepareUI()
        {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }

        // Depending on the ItemData Type, Add appropriate buttons of the items on right click.
        private void HandleItemActionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex, 0));
            }

            if (itemAction.ActionName == "Equip")
            {
                inventoryUI.AddAction("Equip2", () => PerformAction(itemIndex, 1));
            }
            else
            {
                IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
                if (destroyableItem != null)
                {
                    inventoryUI.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
                }
            }
        }

        // If Drop item button is selected, destroy item.
        private void DropItem(int itemIndex, int quantity)
        {
            inventoryData.RemoveItem(itemIndex, quantity);
            inventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);
        }

        // If a button is clicked perform the action of the appropriate button
        // EX: Consume is clicked on a consumable, then perform the Consume function associated with the item
        public void PerformAction(int itemIndex, int equipIndex)
        {
            itemActionPanelObject.Toggle(false);
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState, equipIndex);
                audioSource.PlayOneShot(itemAction.actionSFX);
                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                {
                    inventoryUI.ResetSelection();
                }
            }
        }

        // If Item is dragged create an instance of the selected object attached to cursor
        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        // If Item is dragged onto another item, change the index of the item data
        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            inventoryData.SwapItems(itemIndex_1, itemIndex_2);
        }

        // If item is selected Update the Inventory UI with the appropriate description
        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage,
                item.name, description);
        }

        // Add Item Description to the appropriate format for the UI.
        private string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();
            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} " +
                    $": {inventoryItem.itemState[i].value} / " +
                    $"{inventoryItem.item.DefaultParametersList[i].value}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        // Load Inventory Data
        public void LoadData(GameData data)
        {
            for (int i = 0; i < 18; i++)
            {
                itemsToAdd.Add(InventoryItem.GetEmptyItem());
            }
            for (int i = 0; i < itemsToAdd.Count; i++)
            {
                var temp = new InventoryItem();
                temp.ID = data.inventoryItems[i].ID;
                temp.quantity = data.inventoryItems[i].quantity;
                temp.item = database.GetItem(temp.ID);
                itemsToAdd[i] = temp;
            }
        }

        // Save Inventory Data
        public void SaveData(GameData data)
        {
            data.inventoryItems = inventoryData.inventoryItems;
        }

        // Buy item from shop
        public void BoughtItem(ItemSO itemSO)
        {
            inventoryData.AddItem(itemSO, itemSO.ID, 1);
        }
        
        // Check if player has enough currency
        public bool TrySpendCurrency(int currencyAmount)
        {
            if (playerInventory.GetCoinAmount() >= currencyAmount)
            {
                playerInventory.DecreaseCoins(currencyAmount);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

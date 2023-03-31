using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Inventory.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField] private UIInventoryItem itemPrefab;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private UIInventoryDescription itemDescription;
        [SerializeField] private MouseFollower mouseFollower;
        [SerializeField] private UIInventoryDescription inventoryDescription;
        [SerializeField] private ItemActionPanel actionPanel;
        [SerializeField] private GameObject weaponIconUI;
        [SerializeField]private Animator inventoryMenu;

        private List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();

        private int currentlyDraggedItemIndex = -1;

        public event Action<int> OnDescriptionRequested,
                OnItemActionRequested,
                OnStartDragging;

        public event Action<int, int> OnSwapItems;

        public int currentlySelectedIndex { get; set; }

        private void Awake()
        {
            Hide(true);
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
            gameObject.SetActive(false);
        }

        public void InitializeInventoryUI(int inventorysize)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                UIInventoryItem uiItem =
                    Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                uiItem.transform.SetParent(contentPanel);
                listOfUIItems.Add(uiItem);
                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

        internal void ResetAllItems()
        {
            foreach (var item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();
            listOfUIItems[itemIndex].Select();
            currentlySelectedIndex = itemIndex;
        }

        public void UpdateData(int itemIndex,
            Sprite itemImage, int itemQuantity)
        {
            if (listOfUIItems.Count > itemIndex)
            {
                listOfUIItems[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnItemActionRequested?.Invoke(index);
        }

        private void HandleEndDrag(UIInventoryItem inventoryItemUI)
        {
            ResetDraggedItem();
        }

        private void HandleSwap(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(inventoryItemUI);
        }

        private void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }

        private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
                return;
            currentlyDraggedItemIndex = index;
            HandleItemSelection(inventoryItemUI);
            OnStartDragging?.Invoke(index);
        }

        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }

        private void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
                return;
            OnDescriptionRequested?.Invoke(index);
        }

        public void Show()
        {
            weaponIconUI.SetActive(true);
            gameObject.SetActive(true);
            inventoryMenu.SetBool("start", true);
            ResetSelection();
            SelectFirstItem();
        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription();
            DeselectAllItems();
        }

        public void AddAction(string actionName, Action performAction, bool firstAction = false)
        {
            actionPanel.AddButton(actionName, performAction, firstAction);
        }

        public void ShowItemAction(int itemIndex)
        {
            actionPanel.Toggle(true);
            actionPanel.transform.position = listOfUIItems[itemIndex].transform.position;
        }

        public void ShowItemActionOnInput(int itemIndex)
        {
            if (!listOfUIItems[itemIndex].hasNoActions && !listOfUIItems[itemIndex].empty)
            {
                ShowItemAction(itemIndex);
                OnItemActionRequested?.Invoke(itemIndex);
            }
        }

        private void DeselectAllItems()
        {
            foreach (UIInventoryItem item in listOfUIItems)
            {
                item.Deselect();
            }
            actionPanel.Toggle(false);
        }

        public void Hide(bool awake = false)
        {
            if (!awake)
            {
                weaponIconUI.SetActive(false);
            }
            inventoryMenu.SetBool("start", false);
            actionPanel.Toggle(false);
            ResetDraggedItem();
        }

        public void DisableInventoryMenu()
        {
            weaponIconUI.SetActive(false);
            gameObject.SetActive(false);
        }

        public void SelectFirstItem()
        {
            listOfUIItems[0].Select();
            currentlySelectedIndex = 0;
            if (!listOfUIItems[0].empty)
            {
                HandleItemSelection(listOfUIItems[0]);
            }
            else
            {
                inventoryDescription.ResetDescription();
            }
        }

        public void SelectItemIndex(int index)
        {
            DeselectAllItems();
            listOfUIItems[index].Select();
            currentlySelectedIndex = index;
            if (!listOfUIItems[index].empty)
            {
                HandleItemSelection(listOfUIItems[index]);
            }
            else
            {
                inventoryDescription.ResetDescription();
            }

        }
    }
}

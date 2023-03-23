using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace Project.Inventory.UI
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler,
        IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {
        [SerializeField]
        public Image itemImage;
        [SerializeField]
        private TMP_Text quantityTxt;

        [SerializeField]
        private Image borderImage;

        private InventoryController inventoryController;

        public event Action<UIInventoryItem> OnItemClicked,
            OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag,
            OnRightMouseBtnClick;

        public bool empty = true;
        public bool hasNoActions = false;

        public void Awake()
        {
            ResetData();
            Deselect();
            inventoryController = GameObject.FindWithTag("Player").GetComponent<InventoryController>();
        }

        private void Start()
        {
            CheckHasNoActionItemList();
        }

        private void CheckHasNoActionItemList()
        {
            if (itemImage != null && !empty)
            {
                foreach (Sprite item in inventoryController.itemsWithNoActions)
                {
                    if (itemImage.sprite.texture.GetRawTextureData().SequenceEqual(item.texture.GetRawTextureData()))
                    {
                        hasNoActions = true;
                    }
                }
            }
        }

        public void ResetData()
        {
            itemImage.gameObject.SetActive(false);
            empty = true;
        }
        public void Deselect()
        {
            borderImage.enabled = false;
        }
        public void SetData(Sprite sprite, int quantity)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
            quantityTxt.text = quantity + "";
            empty = false;
        }

        public void Select()
        {
            borderImage.enabled = true;
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else
            {
                OnItemClicked?.Invoke(this);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnItemEndDrag?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (empty)
                return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {

        }
    }
}

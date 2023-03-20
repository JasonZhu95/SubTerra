using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Project.Inventory.Data;

public class ShopConfirmButton : MonoBehaviour
{
    public ShopButtonController shopButtonController;
    public Animator animator;
    public int thisIndex;
    [SerializeField] private ShopButtonController shopMenuButtonController;

    public ItemSO ItemData { get; set; }
    [SerializeField]
    private UI_Shop uiShop;
    [SerializeField]
    private GameObject shopConfirmMenu;

    private PlayerInputHandler inputHandler;


    private void Awake()
    {
        inputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (shopButtonController.index == thisIndex)
        {
            animator.SetBool("selected", true);
            if (inputHandler.MainActionUIInput)
            {
                animator.SetBool("pressed", true);
                if (thisIndex == 0)
                {
                    // TODO: Buy the item
                    inputHandler.MainActionUIInput = false;
                    uiShop.OnItemTemplateClick(ItemData);
                    shopConfirmMenu.SetActive(false);
                    shopMenuButtonController.gameObject.SetActive(true);
                }
                else if (thisIndex == 1)
                {
                    // TODO: Do not buy the item
                    inputHandler.MainActionUIInput = false;
                    shopConfirmMenu.SetActive(false);
                    shopMenuButtonController.gameObject.SetActive(true);
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

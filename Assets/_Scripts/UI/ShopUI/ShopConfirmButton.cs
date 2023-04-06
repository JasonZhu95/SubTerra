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
    [SerializeField] private Animator shopConfirmMenuAnim;
    [SerializeField] private Animator shopConfirmMenuReturnAnim;

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

    private void OnEnable()
    {
        uiShop.gameObject.SetActive(false);
        shopConfirmMenuAnim.SetBool("start", true);
    }

    private void OnDisable()
    {
    }

    private void Update()
    {
        if (shopButtonController.index == thisIndex)
        {
            animator.SetBool("selected", true);
            if (inputHandler.MainActionUIInput)
            {
                FindObjectOfType<SoundManager>().Play("UIClick");
                animator.SetBool("pressed", true);
                if (thisIndex == 0)
                {
                    uiShop.OnItemTemplateClick(ItemData);
                    Hide();
                }
                else if (thisIndex == 1)
                {
                    Hide();
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

    private void Hide()
    {
        inputHandler.MainActionUIInput = false;
        shopConfirmMenuAnim.SetBool("start", false);
    }

    public void DeactivateShopConfirmMenu()
    {
        shopConfirmMenu.SetActive(false);
        uiShop.transform.localScale = new Vector3(0, 0, 0);
        uiShop.gameObject.SetActive(true);
        shopConfirmMenuReturnAnim.SetBool("start", true);
        shopMenuButtonController.shopVerticalLayoutObject.SetActive(true);
    }
}

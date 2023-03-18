using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public ShopButtonController shopButtonController;
    public Animator animator;
    public int thisIndex;
    [SerializeField] GameObject shopPanelToOpen;
    [SerializeField] GameObject shopPanelToClose;

    private void Update()
    {
        if (shopButtonController.index == thisIndex)
        {
            animator.SetBool("selected", true);
            if (shopButtonController.isPressConfirm)
            {
                animator.SetBool("pressed", true);
                if (shopPanelToOpen != null)
                {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAnimationIntermediary : MonoBehaviour
{
    [SerializeField]
    private UI_Shop shopUI;

    private void OnAnimationFinished()
    {
        shopUI.DeactivateShopMenu();
    }
}

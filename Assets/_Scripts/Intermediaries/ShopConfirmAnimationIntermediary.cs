using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopConfirmAnimationIntermediary : MonoBehaviour
{
    [SerializeField]
    private ShopConfirmButton shopConfirmButton;

    private void OnAnimationFinished()
    {
        shopConfirmButton.DeactivateShopConfirmMenu();
    }
}

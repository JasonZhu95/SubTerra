using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopToConfirmIntermediary : MonoBehaviour
{
    [SerializeField]
    private ShopButton shopButton;

    private void OnAnimationFinished()
    {
        shopButton.ActivateConfirmMenu();
    }

    private void OnDisable()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
}

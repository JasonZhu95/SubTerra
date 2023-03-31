using Project.Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryToInventoryPanel : MonoBehaviour
{
    [SerializeField]
    private UIInventoryPage inventoryPage;

    private void OnAnimationFinished()
    {
        inventoryPage.DisableInventoryMenu();
    }
}

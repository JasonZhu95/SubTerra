using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Inventory.Data;
using Project.Inventory.UI;
using Project.Weapons;

public class AgentWeapon : MonoBehaviour
{
    [SerializeField]
    private ItemActionPanel itemActionPanelObject;

    [SerializeField]
    private PlayerInventory playerInventory;

    [SerializeField]
    private InventorySO inventoryData;

    [SerializeField]
    private List<ItemParameter> parametersToModify, itemCurrentState;

    public void SetWeapon(EquippableItemSO weaponItemSO, List<ItemParameter> itemState, int equipIndex)
    {
        if (equipIndex == 0)
        {
            inventoryData.AddItem(playerInventory.weapons[0], playerInventory.weapons[0].ID, 1, itemCurrentState);
            playerInventory.SetWeapon(weaponItemSO as WeaponDataSO, (CombatInputs)0);
            //itemActionPanelObject.Toggle(false);
        }
        else if (equipIndex == 1)
        {
            inventoryData.AddItem(playerInventory.weapons[1], playerInventory.weapons[1].ID, 1, itemCurrentState);
            playerInventory.SetWeapon(weaponItemSO as WeaponDataSO, (CombatInputs)1);
            //itemActionPanelObject.Toggle(false);
        }
        else
        {
            Debug.Log("Equip Index Error Out of Range");
        }

        this.itemCurrentState = new List<ItemParameter>(itemState);
        ModifyParameters();
    }

    private void ModifyParameters()
    {
        foreach (var parameter in parametersToModify)
        {
            if (itemCurrentState.Contains(parameter))
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value + parameter.value;
                itemCurrentState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }
    }
}

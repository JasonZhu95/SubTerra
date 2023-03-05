using Project.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Inventory.Data
{
    [CreateAssetMenu(fileName = "newEquippableItemData", menuName = "Data/Item Data/Equippable")]
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Equip";

        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null, int equipIndex = 0)
        {
            AgentWeapon weaponSystem = character.GetComponent<AgentWeapon>();

            if (weaponSystem != null)
            {
                weaponSystem.SetWeapon(this, itemState == null ? DefaultParametersList : itemState, equipIndex);
                return true;
            }
            return false;
        }
    }
}

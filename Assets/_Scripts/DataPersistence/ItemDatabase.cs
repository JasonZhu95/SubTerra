using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Project.Inventory.Data;

[CreateAssetMenu(menuName = "InventorySystem / Item DataBase")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemSO> itemDatabase;

    [ContextMenu("Set IDs")]
    public void SetItemIds()
    {
        itemDatabase = new List<ItemSO>();

        var foundItems = Resources.LoadAll<ItemSO>("ItemData").OrderBy(i => i.ID).ToList();

        var hasIDInRange = foundItems.Where(i => i.ID != -1 && i.ID < foundItems.Count).OrderBy(i => i.ID).ToList();
        var hasIDNotInRange = foundItems.Where(i => i.ID != -1 && i.ID >= foundItems.Count).OrderBy(i => i.ID).ToList();
        var noID = foundItems.Where(i => i.ID <= -1).ToList();

        var index = 0;
        for (int i = 0; i < foundItems.Count; i++)
        {
            ItemSO itemToAdd;
            itemToAdd = hasIDInRange.Find(d => d.ID == i);

            if (itemToAdd != null)
            {
                itemDatabase.Add(itemToAdd);
            }
            else if (index < noID.Count)
            {
                noID[index].ID = i;
                itemToAdd = noID[index];
                index++;
                itemDatabase.Add(itemToAdd);
            }
        }

        foreach (var item in hasIDNotInRange)
        {
            itemDatabase.Add(item);
        }
    }

    public ItemSO GetItem(int id)
    {
        return itemDatabase.Find(i => i.ID == id);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Inventory.Data;

[System.Serializable]
public class GameData
{
    public int coinCount;
    public int checkPointIndex;
    public SerializableDictionary<string, bool> itemsCollected;
    public List<InventoryItem> inventoryItems;

    public Vector3 checkPointPosition;

    //Default values on game start when tehre is not data to load
    public GameData()
    {
        this.coinCount = 0;
        checkPointIndex = 0;

        // TODO: Set manually to first checkpoint position. Change later
        checkPointPosition = new Vector3(-5.18f, -6.0f, 0);
        inventoryItems = new List<InventoryItem>(new InventoryItem[18]);

        itemsCollected = new SerializableDictionary<string, bool>();
    }
}

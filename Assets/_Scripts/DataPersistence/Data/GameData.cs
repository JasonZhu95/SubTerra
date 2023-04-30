using System.Collections.Generic;
using UnityEngine;
using Project.Inventory.Data;
using System;
using Project.Weapons;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public int coinCount;
    public int checkPointIndex;
    public float currentHealth;
    public float maxHealth;
    public float playTime;

    public bool disableDash;
    public bool disableWallJump;
    public bool disableWallClimb;
    public bool disableWallGrab;
    public bool disableWallSlide;
    public int[] equippedItemsID;

    // Boss Fight Checks;
    public bool rangerBossDefeated;
    public bool templeBossDefeated;
    public bool demonBossDefeated;

    public SerializableDictionary<string, bool> itemsCollected;
    public SerializableDictionary<string, bool> abilityCollected;
    public SerializableDictionary<string, bool> doorState;
    public List<InventoryItem> inventoryItems;

    public Vector3 checkPointPosition;

    //Default values on game start when there is not data to load
    public GameData()
    {
        coinCount = 0;
        checkPointIndex = 0;
        // TODO: Set Manually Max Health values;
        currentHealth = 5f;
        maxHealth = 5f;

        disableDash = true;
        disableWallJump = true;
        disableWallClimb = true;
        disableWallGrab = true;
        disableWallSlide = true;

        rangerBossDefeated = false;
        templeBossDefeated = false;
        demonBossDefeated = false;

        // TODO: Set manually to first checkpoint position. Change later
        checkPointPosition = new Vector3(-18.6f, 7f, 0);
        inventoryItems = new List<InventoryItem>(new InventoryItem[24]);

        itemsCollected = new SerializableDictionary<string, bool>();
        abilityCollected = new SerializableDictionary<string, bool>();
        doorState = new SerializableDictionary<string, bool>();
        equippedItemsID = new int[2] { -1, -1 };
    }

    public string FormatPlayTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(playTime);
        return time.ToString("hh':'mm':'ss");
    }
}

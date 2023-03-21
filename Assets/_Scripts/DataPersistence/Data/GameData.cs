using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Inventory.Data;
using System;

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

        disableDash = false;
        disableWallJump = false;
        disableWallClimb = false;
        disableWallGrab = false;
        disableWallSlide = false;

        // TODO: Set manually to first checkpoint position. Change later
        checkPointPosition = new Vector3(-5.18f, -6.0f, 0);
        inventoryItems = new List<InventoryItem>(new InventoryItem[18]);

        itemsCollected = new SerializableDictionary<string, bool>();
        abilityCollected = new SerializableDictionary<string, bool>();
        doorState = new SerializableDictionary<string, bool>();
    }

    public string FormatPlayTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(playTime);
        return time.ToString("hh':'mm':'ss");
    }
}

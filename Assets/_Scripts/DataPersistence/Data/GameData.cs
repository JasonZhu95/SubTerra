using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int coinCount;
    public int checkPointIndex;
    public SerializableDictionary<string, bool> itemsCollected;


    public Vector3 checkPointPosition;

    //Default values on game start when tehre is not data to load
    public GameData()
    {
        this.coinCount = 0;
        checkPointIndex = 0;

        // TODO: Set manually to first checkpoint position. Change later
        checkPointPosition = new Vector3(-5.18f, -6.0f, 0);

        itemsCollected = new SerializableDictionary<string, bool>();
    }
}

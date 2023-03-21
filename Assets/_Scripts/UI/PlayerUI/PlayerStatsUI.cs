using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField]
    private Stats playerStats;
    [SerializeField]
    private PlayerInventory playerInventory;
    [SerializeField]
    private TextMeshProUGUI coinText;

    public int numHeartContainers;

    [SerializeField]
    private Image[] heartContainers;
    [SerializeField]
    private Sprite fullHeart;
    [SerializeField]
    private Sprite emptyHeart;

    private void Awake()
    {
        numHeartContainers = (int)playerStats.Health.MaxValue;
    }

    private void Update()
    {
        // Update Coin Text
        coinText.text = playerInventory.CoinsHeld.ToString();

        // Assign if a heart should exist or not
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < playerStats.Health.currentValue)
            {
                heartContainers[i].sprite = fullHeart;
            }
            else
            {
                heartContainers[i].sprite = emptyHeart;
            }

            if (i < numHeartContainers)
            {
                heartContainers[i].enabled = true;
            }
            else
            {
                heartContainers[i].enabled = false;
            }
        }
    }
}

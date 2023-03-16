using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField]
    private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI healthCount;
    [SerializeField] private TextMeshProUGUI coinCount;
    [SerializeField] private TextMeshProUGUI playTime;

    [Header("Clear Data Button")]
    [SerializeField] private Button clearButton;

    public bool HasData { get; private set; } = false;

    private Button saveSlotButton;

    private void Awake()
    {
        saveSlotButton = this.GetComponent<Button>();
    }

    public void SetData(GameData data)
    {
        if (data == null)
        {
            HasData = false;
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);
        }
        else
        {
            HasData = true;
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);

            playTime.text = data.FormatPlayTime();
            healthCount.text = "HEALTH COUNT: " + data.currentHealth;
            coinCount.text = "COIN COUNT: " + data.coinCount;
        }
    }

    public string GetProfileId()
    {
        return this.profileId;
    }

    public void SetInteractable(bool interactable)
    {
        saveSlotButton.interactable = interactable;
        clearButton.interactable = interactable;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.MenuUI;
using UnityEngine.SceneManagement;
using Project.LevelSetup;
using UnityEngine.UI;

public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private MenuManager mainMenu;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    private LevelLoaderManager levelLoaderManager;

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = this.GetComponentsInChildren<SaveSlot>();
        levelLoaderManager = GameObject.Find("LevelLoader").GetComponent<LevelLoaderManager>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();
        // Update profile id
        DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        //Create a new game
        if (!isLoadingGame)
        {
            DataPersistenceManager.instance.NewGame();
        }

        levelLoaderManager.LoadNextLevel();
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        // Activate menu on UI
        this.gameObject.SetActive(true);

        this.isLoadingGame = isLoadingGame;

        // Load profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        // Setup save slots in UI
        GameObject firstSelected = backButton.gameObject;
        foreach (SaveSlot saveSlot in saveSlots)
        {
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
                if (firstSelected.Equals(backButton.gameObject))
                {
                    firstSelected = saveSlot.gameObject;
                }
            }
        }
        StartCoroutine(this.SetFirstSelected(firstSelected));
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot saveSlot in saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        backButton.interactable = false;
    }
}

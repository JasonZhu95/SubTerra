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

    [Header("Confirmation Popout")]
    [SerializeField] private ConfirmationPopoutMenu confirmationPopoutMenu;

    private LevelLoaderManager levelLoaderManager;

    private SaveSlot[] saveSlots;

    private bool isLoadingGame = false;

    private void Awake()
    {
        saveSlots = GetComponentsInChildren<SaveSlot>();
        levelLoaderManager = GameObject.Find("LevelLoader").GetComponent<LevelLoaderManager>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();

        // Loading Game
        if (isLoadingGame)
        {
            DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            SaveGameAndLoadScene();
        }
        // New Game, but has data in slot
        else if (saveSlot.HasData)
        {
            confirmationPopoutMenu.ActivateMenu(
                "Starting a New Game with this slot will override the currently saved data.  Are you sure?",
                // Execute function depending on which button is clicked
                () =>
                {
                    DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    DataPersistenceManager.instance.NewGame();
                    SaveGameAndLoadScene();
                },
                () =>
                {
                    ActivateMenu(isLoadingGame);
                });
        }
        // New Game and slot is empty
        else
        {
            DataPersistenceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            DataPersistenceManager.instance.NewGame();
            SaveGameAndLoadScene();
        }

    }

    private void SaveGameAndLoadScene()
    {
        // Save the game when loading a new scene
        DataPersistenceManager.instance.SaveGame();

        // Loads the scene
        levelLoaderManager.LoadNextLevel();
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnClearClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();

        confirmationPopoutMenu.ActivateMenu(
            "Are you sure you want to delete this save file?",
            () => {
                DataPersistenceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
                ActivateMenu(isLoadingGame);
            },
            () => {
                ActivateMenu(isLoadingGame);
            }
        );

    }

    public void ActivateMenu(bool isLoadingGame)
    {
        // Activate menu on UI
        gameObject.SetActive(true);

        this.isLoadingGame = isLoadingGame;

        // Load profiles that exist
        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.instance.GetAllProfilesGameData();

        // ensure that back button is interactable when activated
        backButton.interactable = true;

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
        Button firstSelectedButton = firstSelected.GetComponent<Button>();
        SetFirstSelected(firstSelectedButton);
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
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

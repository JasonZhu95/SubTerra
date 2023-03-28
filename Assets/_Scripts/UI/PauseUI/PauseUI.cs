using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Project.LevelSetup;

public class PauseUI : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject firstSelectedButton;

    private Animator anim;
    private LevelLoaderManager levelLoaderManager;

    private bool isPaused;

    public PlayerInputHandler InputHandler { get; private set; }


    private void Awake()
    {
        pauseMenu.SetActive(false);
        anim = pauseMenu.GetComponent<Animator>();
        InputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
        levelLoaderManager = GameObject.Find("LevelLoader").GetComponent<LevelLoaderManager>();
    }

    private void Update()
    {
        if (InputHandler.PausePressed)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        InputHandler.SwitchToActionMap("UI");
        InputHandler.PausePressed = false;
        anim.SetBool("pause", true);
        isPaused = true;
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void ResumeGame()
    {
        anim.SetBool("pause", false);
        InputHandler.PausePressed = false;
        InputHandler.BlockActionInput = false;
        InputHandler.SwitchToActionMap("Gameplay");
        isPaused = false;
    }

    public void ReturnToMenu()
    {
        DataPersistenceManager.instance.SaveGame();
        InputHandler.BlockActionInput = false;
        levelLoaderManager.LoadMainMenu();
        InputHandler.SwitchToActionMap("Gameplay");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

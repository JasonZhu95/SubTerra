using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Project.LevelSetup;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject confirmationMenu;

    [SerializeField]
    private GameObject firstSelectedButton;

    [SerializeField]
    private GameObject firstSelectedConfirmationButton;

    private Animator anim;
    private Animator animConfirmation;
    private LevelLoaderManager levelLoaderManager;
    private DataPersistenceManager dataPersistenceManager;

    private bool isPaused;

    public PlayerInputHandler InputHandler { get; private set; }

    private Color newColor;
    private Image pauseMenuImage;

    private void Awake()
    {
        pauseMenu.SetActive(false);
        anim = pauseMenu.GetComponent<Animator>();
        animConfirmation = confirmationMenu.GetComponent<Animator>();
        InputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
        levelLoaderManager = GameObject.Find("LevelLoader").GetComponent<LevelLoaderManager>();
        pauseMenuImage = pauseMenu.GetComponent<Image>();
        newColor = pauseMenuImage.color;
        dataPersistenceManager = GameObject.Find("DataPersistenceManager").GetComponent<DataPersistenceManager>();
    }

    private void Update()
    {
        if (InputHandler.PausePressed)
        {
            if (pauseMenu.activeSelf)
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
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void ResumeGame()
    {
        FindObjectOfType<SoundManager>().Play("UIClick");
        anim.SetBool("pause", false);
        anim.SetBool("pauseFromConfirm", false);
        InputHandler.PausePressed = false;
        InputHandler.BlockActionInput = false;
        InputHandler.SwitchToActionMap("Gameplay");
    }

    public void StartConfirmationMenu()
    {
        FindObjectOfType<SoundManager>().Play("UIClick");
        pauseMenu.SetActive(false);
        confirmationMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedConfirmationButton);
        animConfirmation.SetBool("start", true);
    }

    public void CancelConfirmationMenu()
    {
        FindObjectOfType<SoundManager>().Play("UIClick");
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        pauseMenu.SetActive(true);
        anim.SetBool("pauseFromConfirm", true);
        animConfirmation.SetBool("start", false);
        confirmationMenu.SetActive(false);
    }

    public void ReturnToMenu()
    {
        FindObjectOfType<SoundManager>().Play("UIClick");
        InputHandler.BlockActionInput = false;
        InputHandler.SwitchToActionMap("Gameplay");
        dataPersistenceManager.SaveGame();
        levelLoaderManager.LoadMainMenu();
    }

    public void QuitGame()
    {
        FindObjectOfType<SoundManager>().Play("UIClick");
        Application.Quit();
    }
}

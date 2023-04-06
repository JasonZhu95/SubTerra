using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenuSelect : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private GameObject graphicsDialogueMenu;
    [SerializeField] private GameObject soundDialogueMenu;
    [SerializeField] private GameObject gameplayDialogueMenu;

    private Animator optionsMenuAnim;

    enum ButtonClicked { Graphics, Sound, Gameplay, Back}

    private ButtonClicked lastButtonClicked;

    private void Awake()
    {
        optionsMenuAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
        optionsMenuAnim.SetBool("start", true);
    }

    public void OnBackButtonClicked()
    {
        optionsMenuAnim.SetBool("start", false);
        FindObjectOfType<SoundManager>().Play("UIClick");
        lastButtonClicked = ButtonClicked.Back;
    }

    public void OnGraphicsButtonClicked()
    {
        optionsMenuAnim.SetBool("start", false);
        FindObjectOfType<SoundManager>().Play("UIClick");
        lastButtonClicked = ButtonClicked.Graphics;
    }

    public void OnSoundButtonClicked()
    {
        optionsMenuAnim.SetBool("start", false);
        FindObjectOfType<SoundManager>().Play("UIClick");
        lastButtonClicked = ButtonClicked.Sound;
    }

    public void OnGameplayButtonClicked()
    {
        optionsMenuAnim.SetBool("start", false);
        FindObjectOfType<SoundManager>().Play("UIClick");
        lastButtonClicked = ButtonClicked.Gameplay;
    }

    private void ActivateSettingsMenu()
    {
        if (lastButtonClicked == ButtonClicked.Graphics)
        {
            graphicsDialogueMenu.SetActive(true); 
            gameObject.SetActive(false);
        }
        else if (lastButtonClicked == ButtonClicked.Sound)
        {
            soundDialogueMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else if (lastButtonClicked == ButtonClicked.Gameplay)
        {
            gameplayDialogueMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        else if (lastButtonClicked == ButtonClicked.Back)
        {
            DisableOptionsMenu();
        }
    }

    public void DisableOptionsMenu()
    {
        for (int i = 0; i < mainMenuContainer.transform.childCount; i++)
        {
            mainMenuContainer.transform.GetChild(i).gameObject.SetActive(true);
        }
        mainMenuContainer.SetActive(false);
        mainMenuContainer.SetActive(true);
        gameObject.SetActive(false);
    }
}

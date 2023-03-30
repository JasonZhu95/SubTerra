using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplayMenuUI : MonoBehaviour
{
    private Animator gameplayMenuAnim;

    [SerializeField] private GameObject firstSelected;
    [SerializeField] private GameObject optionsMenu;

    private void Awake()
    {
        gameplayMenuAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
        gameplayMenuAnim.SetBool("start", true);
    }

    public void OnBackClicked()
    {
        gameplayMenuAnim.SetBool("start", false);
    }

    public void DeactivateMenu()
    {
        optionsMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}

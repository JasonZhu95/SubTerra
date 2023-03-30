using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundMenuUI : MonoBehaviour
{
    private Animator soundMenuAnim;
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private GameObject optionsMenu;

    private void Awake()
    {
        soundMenuAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
        soundMenuAnim.SetBool("start", true);
    }

    public void OnBackClicked()
    {
        soundMenuAnim.SetBool("start", false);
    }

    public void DeactivateMenu()
    {
        optionsMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}

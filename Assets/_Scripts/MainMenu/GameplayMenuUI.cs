using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayMenuUI : MonoBehaviour
{
    private Animator gameplayMenuAnim;

    [SerializeField]
    private GameObject optionsMenu;

    private void Awake()
    {
        gameplayMenuAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
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

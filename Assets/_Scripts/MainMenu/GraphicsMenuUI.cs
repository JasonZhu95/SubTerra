using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GraphicsMenuUI : MonoBehaviour
{
    private Animator graphicsMenuAnim;

    [SerializeField] private GameObject firstSelected;
    [SerializeField] private GameObject optionsMenu;

    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
        graphicsMenuAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        graphicsMenuAnim.SetBool("start", true);
    }

    public void OnBackClicked()
    {
        graphicsMenuAnim.SetBool("start", false);
    }

    public void DeactivateMenu()
    {
        optionsMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}

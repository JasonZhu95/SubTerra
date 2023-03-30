using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsMenuUI : MonoBehaviour
{
    private Animator graphicsMenuAnim;

    [SerializeField]
    private GameObject optionsMenu;

    private void Awake()
    {
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

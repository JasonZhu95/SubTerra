using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMenuUI : MonoBehaviour
{
    private Animator soundMenuAnim;

    [SerializeField]
    private GameObject optionsMenu;

    private void Awake()
    {
        soundMenuAnim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
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

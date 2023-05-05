using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;

public class SwordTutorialTrigger : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Animator tutorialAnim;
    private PlayerInputHandler inputHandler;

    private bool swordTutorialActivated = false;
    private bool canvasIsActive = false;

    private void Awake()
    {
        inputHandler = GameObject.FindWithTag("Player").GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (canvasIsActive)
        {
            if (inputHandler.MainActionUIInput)
            {
                tutorialAnim.SetBool("start", false);
                inputHandler.MainActionUIInput = false;
                StartCoroutine(ReEnableGameplay());
                canvasIsActive = false;
            }
        }
    }

    private IEnumerator ReEnableGameplay()
    {
        yield return new WaitForSeconds(.5f);
        inputHandler.SwitchToActionMap("Gameplay");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !swordTutorialActivated)
        {
            tutorialAnim.SetBool("start", true);
            StartCoroutine(SetCanvasBoolAfterAnimation());
            inputHandler.SwitchToActionMap("UINoPause");
            swordTutorialActivated = true;
        }
    }


    private IEnumerator SetCanvasBoolAfterAnimation()
    {
        yield return new WaitForSeconds(.5f);
        canvasIsActive = true;
    }

    public void LoadData(GameData data)
    {
        swordTutorialActivated = data.swordTutorialActivated;
    }

    public void SaveData(GameData data)
    {
        data.swordTutorialActivated = swordTutorialActivated;
    }
}

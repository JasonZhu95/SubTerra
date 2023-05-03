using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;

public class FirstCheckpointTutorial : MonoBehaviour, IDataPersistence
{
    [SerializeField] private Animator tutorialAnim;
    private PlayerInputHandler inputHandler;

    private bool firstCheckPointActivated = false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !firstCheckPointActivated)
        {
            tutorialAnim.SetBool("start", true);
            StartCoroutine(SetCanvasBoolAfterAnimation());
            inputHandler.SwitchToActionMap("UINoPause");
            firstCheckPointActivated = true;
        }
    }

    private IEnumerator SetCanvasBoolAfterAnimation()
    {
        yield return new WaitForSeconds(.5f);
        canvasIsActive = true;
    }

    private IEnumerator ReEnableGameplay()
    {
        yield return new WaitForSeconds(.5f);
        inputHandler.SwitchToActionMap("Gameplay");
    }

    public void LoadData(GameData data)
    {
        firstCheckPointActivated = data.firstCheckpointTriggered;
    }

    public void SaveData(GameData data)
    {
        data.firstCheckpointTriggered = firstCheckPointActivated;
    }
}

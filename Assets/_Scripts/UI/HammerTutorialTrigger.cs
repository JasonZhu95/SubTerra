using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTutorialTrigger : MonoBehaviour
{
    [SerializeField] private Animator tutorialAnim;
    private PlayerInputHandler inputHandler;

    private bool hammerTutorialActivated = false;
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
        if (collision.gameObject.CompareTag("Player") && !hammerTutorialActivated)
        {
            tutorialAnim.SetBool("start", true);
            StartCoroutine(SetCanvasBoolAfterAnimation());
            inputHandler.SwitchToActionMap("UINoPause");
            hammerTutorialActivated = true;
        }
    }


    private IEnumerator SetCanvasBoolAfterAnimation()
    {
        yield return new WaitForSeconds(.5f);
        canvasIsActive = true;
    }

    public void LoadData(GameData data)
    {
        hammerTutorialActivated = data.hammerTutorialActivated;
    }

    public void SaveData(GameData data)
    {
        data.hammerTutorialActivated = hammerTutorialActivated;
    }
}

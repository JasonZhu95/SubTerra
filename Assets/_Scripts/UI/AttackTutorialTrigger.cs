using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTutorialTrigger : MonoBehaviour
{
    [SerializeField] private Animator tutorialAnim;
    private PlayerInputHandler inputHandler;

    private bool attackTutorialActivated = false;
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
        inputHandler.BlockActionInput = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !attackTutorialActivated)
        {
            tutorialAnim.SetBool("start", true);
            StartCoroutine(SetCanvasBoolAfterAnimation());
            inputHandler.SwitchToActionMap("UINoPause");
            inputHandler.BlockActionInput = true;
            attackTutorialActivated = true;
        }
    }


    private IEnumerator SetCanvasBoolAfterAnimation()
    {
        yield return new WaitForSeconds(.5f);
        canvasIsActive = true;
    }

    public void LoadData(GameData data)
    {
        attackTutorialActivated = data.attackTutorialActivated;
    }

    public void SaveData(GameData data)
    {
        data.attackTutorialActivated = attackTutorialActivated;
    }
}

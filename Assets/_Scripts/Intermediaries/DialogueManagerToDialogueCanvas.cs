using Project.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManagerToDialogueCanvas : MonoBehaviour
{
    [SerializeField]
    private DialogueManager dialogueManager;

    private void OnAnimationFinished()
    {
        dialogueManager.ExitDialogueMode();
    }
}

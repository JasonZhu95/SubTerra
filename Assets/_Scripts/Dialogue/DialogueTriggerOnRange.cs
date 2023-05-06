using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;

namespace Project.UI
{
    public class DialogueTriggerOnRange : MonoBehaviour
    {
        private DialogueManager dialogueManagerReference;

        [Header("Ink JSON")]
        [SerializeField] private TextAsset inkJSON;

        private bool playerInRange;
        public bool DialogueHasBeenPlayed { get; set; }

        public bool interactInputPressed { get; set; }
        public bool rangerDialogue = false;
        public bool guardianDialogue = false;

        private void Awake()
        {
            dialogueManagerReference = GameObject.FindWithTag("DialogueContainer").GetComponent<DialogueManager>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                dialogueManagerReference.OnDialogueFinish += DialogueHasFinished;
                playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                dialogueManagerReference.OnDialogueFinish -= DialogueHasFinished;
                playerInRange = false;
            }
        }

        private void Update()
        {
            if (playerInRange && !dialogueManagerReference.DialogueIsPlaying && !DialogueHasBeenPlayed)
            {
                dialogueManagerReference.EnterDialogueMode(inkJSON);
            }
        }

        public void SetDialogueHasBeenPlayed(bool hasBeenPlayed)
        {
            DialogueHasBeenPlayed = hasBeenPlayed;
            if (rangerDialogue)
            {
                FindObjectOfType<SoundManager>().Play("BossRangerTheme");
                FindObjectOfType<SoundManager>().StopPlay("MusicTheme1");
            }
            if (guardianDialogue)
            {
                FindObjectOfType<SoundManager>().Play("BossGuardianTheme");
                FindObjectOfType<SoundManager>().StopPlay("MusicTheme1");
            }
        }

        public void DialogueHasFinished()
        {
            SetDialogueHasBeenPlayed(true);
        }
    }
}

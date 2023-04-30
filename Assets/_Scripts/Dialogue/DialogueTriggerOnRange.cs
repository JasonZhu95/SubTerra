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

        private void Awake()
        {
            dialogueManagerReference = GameObject.FindWithTag("DialogueContainer").GetComponent<DialogueManager>();
        }

        private void OnEnable()
        {
            dialogueManagerReference.OnDialogueFinish += DialogueHasFinished;
        }

        private void OnDisable()
        {
            dialogueManagerReference.OnDialogueFinish -= DialogueHasFinished;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
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
        }

        public void DialogueHasFinished()
        {
            SetDialogueHasBeenPlayed(true);
        }
    }
}

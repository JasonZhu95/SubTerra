using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Combat.Interfaces;

namespace Project.UI
{
    public class DialogueTrigger : MonoBehaviour, IInteractable
    {
        private GameObject player;
        private PlayerInputHandler inputHandler;
        private GameObject dialogueManager;
        private DialogueManager dialogueManagerReference;

        [Header("Visual Cue")]
        [SerializeField] private GameObject visualCue;

        [Header("Ink JSON")]
        [SerializeField] private TextAsset inkJSON;

        private bool playerInRange;

        public bool interactInputPressed { get; set; }
    

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            inputHandler = player.GetComponent<PlayerInputHandler>();
            dialogueManager = GameObject.FindWithTag("DialogueContainer");
            dialogueManagerReference = dialogueManager.GetComponent<DialogueManager>();
            visualCue.SetActive(false);
        }

        private void Update()
        {
            if (inputHandler.InteractPressed && visualCue.activeSelf)
            {
                dialogueManagerReference.EnterDialogueMode(inkJSON);
                inputHandler.InteractPressed = false;
            }
        }

        public object GetInteractionContext()
        {
            return null;
        }

        public void SetContext(object obj)
        {

        }

        public void EnableInteraction()
        {
            visualCue.SetActive(true);
        }
    
        public void DisableInteraction()
        {
            visualCue.SetActive(false);
        }

        public Vector3 GetPosition() => transform.position;

    }
}

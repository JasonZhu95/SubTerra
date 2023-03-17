using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Interfaces;
using Project.EventChannels;
using Project.Managers;

namespace Project.UI
{
    public class DialogueTrigger : MonoBehaviour, IInteractable
    {
        private GameObject player;
        private PlayerInputHandler inputHandler;
        private GameObject dialogueManager;
        private DialogueManager dialogueManagerReference;
        [SerializeField] private GameStateEventChannel GameStateEventChannel;

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
            if (playerInRange && !dialogueManagerReference.DialogueIsPlaying)
            {
                visualCue.SetActive(true);
                if (inputHandler.InteractPressed)
                {
                    dialogueManagerReference.EnterDialogueMode(inkJSON);
                    GameStateEventChannel.RaiseSetChangeGameStateEvent(this, new GameStateEventArgs(GameState.UI));
                    inputHandler.InteractPressed = false;
                }
            }
            else
            {
                visualCue.SetActive(false);
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
            playerInRange = true;
        }
    
        public void DisableInteraction()
        {
            playerInRange = false;
        }

        public Vector3 GetPosition() => transform.position;

    }
}

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
        private CollisionSenses playerCollision;
        private PlayerInputHandler inputHandler;
        private GameObject dialogueManager;
        private DialogueManager dialogueManagerReference;

        [Header("Visual Cue")]
        [SerializeField] private GameObject visualCue;

        [Header("Ink JSON")]
        [SerializeField] private TextAsset inkJSON;

        public bool playerInRange;

        public bool interactInputPressed { get; set; }
    
        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            inputHandler = player.GetComponent<PlayerInputHandler>();
            dialogueManager = GameObject.FindWithTag("DialogueContainer");
            dialogueManagerReference = dialogueManager.GetComponent<DialogueManager>();
            visualCue.SetActive(false);
        }

        private void Start()
        {
            playerCollision = player.GetComponent<Player>().Core.transform.GetChild(1).GetComponent<CollisionSenses>();
        }

        private void Update()
        {
            if (playerInRange && !dialogueManagerReference.DialogueIsPlaying && playerCollision.Ground)
            {
                visualCue.SetActive(true);
                if (inputHandler.InteractPressed)
                {
                    dialogueManagerReference.EnterDialogueMode(inkJSON);
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

        public void ChangeDialogueFile(TextAsset inkJSONToChange)
        {
            inkJSON = inkJSONToChange;
        }

        public Vector3 GetPosition() => transform.position;

    }
}

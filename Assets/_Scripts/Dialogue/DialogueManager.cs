using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

namespace Project.UI
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Dialogue UI")]

        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;

        private Story currentStory;
        private GameObject player;
        private PlayerInputHandler inputHandler;

        public bool DialogueIsPlaying { get; private set; }

        private static DialogueManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More than 1 Dialogue MAnager");
            }
            instance = this;
            player = GameObject.FindWithTag("Player");
            inputHandler = player.GetComponent<PlayerInputHandler>();
        }
        private void Start()
        {
            DialogueIsPlaying = false;
            dialoguePanel.SetActive(false);
        }

        private void Update()
        {
            if (!DialogueIsPlaying)
            {
                return;
            }

            if (inputHandler.InteractPressed)
            {
                inputHandler.InteractPressed = false;
                ContinueStory();
            }
        }

        public void EnterDialogueMode(TextAsset inkJSON)
        {
            currentStory = new Story(inkJSON.text);
            DialogueIsPlaying = true;
            dialoguePanel.SetActive(true);

            ContinueStory();
        }

        private void ExitDialogueMode()
        {
            DialogueIsPlaying = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
        }

        private void ContinueStory()
        {
            if (currentStory.canContinue)
            {
                dialogueText.text = currentStory.Continue();
            }
            else
            {
                ExitDialogueMode();
            }
        }
    }
}

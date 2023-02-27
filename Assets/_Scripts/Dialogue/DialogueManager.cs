using UnityEngine;
using TMPro;
using Ink.Runtime;
using Project.EventChannels;
using Project.Managers;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;

namespace Project.UI
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("Dialogue UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;

        [SerializeField] private GameStateEventChannel GameStateEventChannel;

        [Header("Choices UI")]
        [SerializeField] private GameObject[] choices;
        private TextMeshProUGUI[] choicesText;

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

            choicesText = new TextMeshProUGUI[choices.Length];
            int index = 0;
            foreach (GameObject choice in choices)
            {
                choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
                index++;
            }
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
            GameStateEventChannel.RaiseSetChangeGameStateEvent(this, new GameStateEventArgs(GameState.Gameplay));
        }

        private void ContinueStory()
        {
            if (currentStory.canContinue)
            {
                dialogueText.text = currentStory.Continue();
                DisplayChoices();
            }
            else
            {
                ExitDialogueMode();
            }
        }

        private void DisplayChoices()
        {
            List<Choice> currentChoices = currentStory.currentChoices;

            if (currentChoices.Count > choices.Length)
            {
                Debug.Log("Too many choices!  Number of choices given: " + currentChoices.Count);
            }

            int index = 0;
            foreach (Choice choice in currentChoices)
            {
                choices[index].gameObject.SetActive(true);
                choicesText[index].text = choice.text;
                index++;
            }

            for (int i = index; i < choices.Length; i++)
            {
                choices[i].gameObject.SetActive(false);
            }

            StartCoroutine(SelectFirstChoice());
        }

        private IEnumerator SelectFirstChoice()
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
        }

        public void MakeChoice(int choiceIndex)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
        }
    }
}

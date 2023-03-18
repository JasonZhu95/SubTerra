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
        #region Serialized Variables
        [Header("Dialogue UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI displayNameText;
        [SerializeField] private Animator portraitAnimator;

        [SerializeField] private GameStateEventChannel GameStateEventChannel;

        [Header("Choices UI")]
        [SerializeField] private GameObject[] choices;
        #endregion

        #region Local Variables
        private TextMeshProUGUI[] choicesText;

        private Story currentStory;
        private GameObject player;
        private PlayerInputHandler inputHandler;
        private Animator layoutAnimator;

        public bool DialogueIsPlaying { get; private set; }

        private static DialogueManager instance;
        #endregion

        #region Ink Parsing
        private const string SPEAKER_TAG = "speaker";
        private const string PORTRAIT_TAG = "portrait";
        private const string LAYOUT_TAG = "layout";
        #endregion

        #region Unity Callback Functions
        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("More than 1 Dialogue MAnager");
            }
            instance = this;
            player = GameObject.FindWithTag("Player");
            inputHandler = player.GetComponent<PlayerInputHandler>();
            layoutAnimator = dialoguePanel.GetComponent<Animator>();
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
        #endregion

        #region Dialogue Functions
        public void EnterDialogueMode(TextAsset inkJSON)
        {
            currentStory = new Story(inkJSON.text);
            DialogueIsPlaying = true;
            dialoguePanel.SetActive(true);
            inputHandler.SwitchToActionMap("UI");

            displayNameText.text = "???";
            portraitAnimator.Play("PortraitDefault");
            layoutAnimator.Play("right");

            ContinueStory();
        }

        private void ExitDialogueMode()
        {
            DialogueIsPlaying = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
            inputHandler.SwitchToActionMap("Gameplay");
        }

        private void ContinueStory()
        {
            if (currentStory.canContinue)
            {
                dialogueText.text = currentStory.Continue();
                DisplayChoices();
                HandleTags(currentStory.currentTags);
            }
            else
            {
                ExitDialogueMode();
            }
        }

        private void HandleTags(List<string> currentTags)
        {
            foreach (string tag in currentTags)
            {
                string[] splitTag = tag.Split(':');
                if (splitTag.Length != 2)
                {
                    Debug.LogError("SPLIT Error Parsing Tag: " + tag);
                }
                string tagKey = splitTag[0].Trim();
                string tagValue = splitTag[1].Trim();

                switch (tagKey)
                {
                    case SPEAKER_TAG:
                        displayNameText.text = tagValue;
                        break;
                    case PORTRAIT_TAG:
                        portraitAnimator.Play(tagValue);
                        break;
                    case LAYOUT_TAG:
                        layoutAnimator.Play(tagValue);
                        break;
                    default:
                        Debug.LogWarning("TAG Error Parsing Tag: " + tag);
                        break;
                }
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
        #endregion
    }
}

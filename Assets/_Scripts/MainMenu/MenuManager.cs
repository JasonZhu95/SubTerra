using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Project.LevelSetup;
using UnityEngine.EventSystems;

namespace Project.MenuUI
{
    public class MenuManager : Menu
    {
        #region Variables
        [Header("Menu Navigation")]
        [SerializeField] private SaveSlotsMenu saveSlotsMenu;
        [SerializeField] private GameObject optionsMenu;

        [Header("Menu Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;

        [Header("Volume Setting")]
        [SerializeField] private TMP_Text volumeTextValue;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private float defaultVolume = 1f;

        [Header("Gameplay Settings")]
        [SerializeField] private TMP_Text ControllerSenTextValue;
        [SerializeField] private Slider controllerSenSlider;
        [SerializeField] private int defaultSen = 4;
        public int mainControllerSen = 4;

        [Header("Toggle Settings")]
        [SerializeField] private Toggle invertYToggle;

        [Header("Graphics Settings")]
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private TMP_Text brightnessTextValue;
        [SerializeField] private float defaultBrightness = 1f;

        [Space(10)]
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private Toggle fullscreenToggle;

        private int qualityLevel;
        private bool isFullscreen;
        private float brightnessLevel;

        [Header("Confirmation")]
        [SerializeField] private GameObject confirmationPrompt;
        //private string levelToLoad;

        [Header("Resolution Dropdowns")]
        public TMP_Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        private Animator mainMenuAnim;
        private bool isOptionsMenuPressed = false;
        #endregion

        #region Unity Callback Functions

        private void Awake()
        {
            mainMenuAnim = GetComponent<Animator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            mainMenuAnim.SetBool("start", true);
            isOptionsMenuPressed = false;
        }
        private void Start()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            DisableButtonsDependingOnProfileData();
        }
        #endregion

        #region Level Load Functions
        // Creates a new game - initialize save data
        public void OnNewGameClicked()
        {
            mainMenuAnim.SetBool("start", false);
            saveSlotsMenu.ActivateMenu(false);
        }

        public void OnLoadGameClicked()
        {
            mainMenuAnim.SetBool("start", false);
            saveSlotsMenu.ActivateMenu(true);
        }
        #endregion

        #region Apply Functions
        public void GameplayApply()
        {
            if (invertYToggle.isOn)
            {
                PlayerPrefs.SetInt("masterInvertY", 1);
                //TODO: Invert Y
            }
            else
            {
                PlayerPrefs.SetInt("masterInvertY", 0);
                //TODO: Uninvert Y
            }

            PlayerPrefs.SetFloat("masterSen", mainControllerSen);
            StartCoroutine(ConfirmationBox());
        }

        public void GraphicsApply()
        {
            PlayerPrefs.SetFloat("masterBrightness", brightnessLevel);
            //TODO:Change Brightness

            PlayerPrefs.SetInt("masterQuality", qualityLevel);
            QualitySettings.SetQualityLevel(qualityLevel);

            PlayerPrefs.SetInt("masterFullscreen", (isFullscreen ? 1 : 0));
            Screen.fullScreen = isFullscreen;

            StartCoroutine(ConfirmationBox());
        }

        public void VolumeApply()
        {
            PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
            StartCoroutine(ConfirmationBox());
        }
        #endregion

        #region Set Functions
        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
            volumeTextValue.text = volume.ToString("0.0");
        }

        public void SetControllerSen(float sensitivity)
        {
            mainControllerSen = Mathf.RoundToInt(sensitivity);
            ControllerSenTextValue.text = sensitivity.ToString("0");
        }

        public void SetBrightness(float brightness)
        {
            brightnessLevel = brightness;
            brightnessTextValue.text = brightness.ToString("0.0");
        }

        public void SetFullScreen(bool _isFullscreen)
        {
            isFullscreen = _isFullscreen;
        }

        public void SetQuality(int qualityIndex)
        {
            qualityLevel = qualityIndex;
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        #endregion

        #region Button Functions
        public void ExitButton()
        {
            Application.Quit();
        }

        public void ResetButton(string MenuType)
        {
            if (MenuType == "Audio")
            {
                AudioListener.volume = defaultVolume;
                volumeSlider.value = defaultVolume;
                volumeTextValue.text = defaultVolume.ToString("0.0");
                VolumeApply();
            }

            if (MenuType == "Gameplay")
            {
                ControllerSenTextValue.text = defaultSen.ToString("0");
                controllerSenSlider.value = defaultSen;
                mainControllerSen = defaultSen;
                invertYToggle.isOn = false;
                GameplayApply();
            }

            if (MenuType == "Graphics")
            {
                //TODO: Reset Brightness value
                brightnessSlider.value = defaultBrightness;
                brightnessTextValue.text = defaultBrightness.ToString("0.0");

                qualityDropdown.value = 1;
                QualitySettings.SetQualityLevel(1);
                fullscreenToggle.isOn = false;
                Screen.fullScreen = false;

                Resolution currentResolution = Screen.currentResolution;
                Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
                resolutionDropdown.value = resolutions.Length;
                GraphicsApply();
            }
        }

        public IEnumerator ConfirmationBox()
        {
            confirmationPrompt.SetActive(true);
            yield return new WaitForSeconds(2);
            confirmationPrompt.SetActive(false);
        }

        private void DisableMenuButtons()
        {
            newGameButton.interactable = false;
        }

        public void ActivateMenu()
        {
            this.gameObject.SetActive(true);
            DisableButtonsDependingOnProfileData();
        }

        public void DeactivateMenu()
        {
            if (isOptionsMenuPressed)
            {
                optionsMenu.SetActive(true);
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        private void DisableButtonsDependingOnProfileData()
        {
            if (!DataPersistenceManager.instance.HasGameData())
            {
                loadGameButton.interactable = false;
            }
        }

        public void OnOptionsMenuButton()
        {
            isOptionsMenuPressed = true;
            mainMenuAnim.SetBool("start", false);
        }

        public void onExitButton()
        {
            Application.Quit();
        }
        #endregion
    }
}

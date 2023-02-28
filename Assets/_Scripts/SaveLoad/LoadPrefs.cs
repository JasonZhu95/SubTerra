using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Project.Menu;

namespace Project.Save
{
    public class LoadPrefs : MonoBehaviour
    {
        [Header("General Setting")]
        [SerializeField] private bool canUse = false;
        [SerializeField] private MenuManager menuController;

        [Header("Volume Setting")]
        [SerializeField] private TMP_Text volumeTextValue;
        [SerializeField] private Slider volumeSlider;

        [Header("Brightness Setting")]
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private TMP_Text brightnessTextValue;

        [Header("Quality Level Setting")]
        [SerializeField] private TMP_Dropdown qualityDropdown;

        [Header("Fullscreen Setting")]
        [SerializeField] private Toggle fullscreenToggle;

        [Header("Sensitivity Setting")]
        [SerializeField] private TMP_Text ControllerSenTextValue;
        [SerializeField] private Slider controllerSenSlider;

        [Header("Invert Y Setting")]
        [SerializeField] private Toggle invertYToggle;

        private void Awake()
        {
            if (canUse)
            {
                if (PlayerPrefs.HasKey("masterVolume"))
                {
                    float localVolume = PlayerPrefs.GetFloat("masterVolume");

                    volumeTextValue.text = localVolume.ToString("0.0");
                    volumeSlider.value = localVolume;
                    AudioListener.volume = localVolume;
                }
                else
                {
                    menuController.ResetButton("Audio");
                }

                if (PlayerPrefs.HasKey("masterQuality"))
                {
                    int localQuality = PlayerPrefs.GetInt("masterQuality");
                    qualityDropdown.value = localQuality;
                    QualitySettings.SetQualityLevel(localQuality);
                }

                if (PlayerPrefs.HasKey("masterFullscreen"))
                {
                    int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");

                    if (localFullscreen == 1)
                    {
                        Screen.fullScreen = true;
                        fullscreenToggle.isOn = true;
                    }
                    else
                    {
                        Screen.fullScreen = false;
                        fullscreenToggle.isOn = false;
                    }
                }

                if (PlayerPrefs.HasKey("masterBrightness"))
                {
                    float localBrightness = PlayerPrefs.GetFloat("masterBrightness");

                    brightnessTextValue.text = localBrightness.ToString("0.0");
                    brightnessSlider.value = localBrightness;

                    //TODO: Change brightness
                }

                if (PlayerPrefs.HasKey("masterSen"))
                {
                    float localSensitivity = PlayerPrefs.GetFloat("masterSen");

                    ControllerSenTextValue.text = localSensitivity.ToString("0");
                    controllerSenSlider.value = localSensitivity;
                    menuController.mainControllerSen = Mathf.RoundToInt(localSensitivity);
                }

                if (PlayerPrefs.HasKey("masterInvertY"))
                {
                    if (PlayerPrefs.GetInt("masterInvertY") == 1)
                    {
                        invertYToggle.isOn = true;
                    }
                    else
                    {
                        invertYToggle.isOn = false;
                    }
                }
            }
        }
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Sliders")]
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;
    public UnityEngine.UI.Slider voiceSlider;
    public UnityEngine.UI.Slider ambientSlider;
    public UnityEngine.UI.Slider sensitivitySlider;

    [Header("Toggles")]
    public UnityEngine.UI.Toggle vsyncToggle;

    [Header("Display")]
    public TMP_Dropdown fullscreenDropdown;
    public TMP_Dropdown resolutionDropdown;

    public UnityEngine.UI.Button resumeButton;
    public UnityEngine.UI.Button mainMenuButton;

    public static bool SettingsIsOpen = false;

    private AudioClip testSound;

    private void Awake()
    {
        testSound = Resources.Load<AudioClip>($"{Constants.sfxPath}/ding-36029");

        if (SceneManager.GetActiveScene().name == Constants.mainMenuSceneString)
        {
            resumeButton.enabled = false;
        }
        else
        {
            resumeButton.enabled = true;
        }
    }


    private void Start()
    {
        // Load saved values into sliders
        musicSlider.value = GameSettings.Instance.musicVolume;
        sfxSlider.value = GameSettings.Instance.sfxVolume;
        voiceSlider.value = GameSettings.Instance.voiceVolume;
        ambientSlider.value = GameSettings.Instance.ambientVolume;
        sensitivitySlider.value = GameSettings.Instance.mouseSensitivity;
        vsyncToggle.isOn = GameSettings.Instance.vsync;

        // Toggle
        vsyncToggle.onValueChanged.AddListener(OnVsyncToggleChanged);

        // Display
        PopulateFullscreenDropdown();
        PopulateResolutionDropdown();

        // Add listeners
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        voiceSlider.onValueChanged.AddListener(OnVoiceChanged);
        ambientSlider.onValueChanged.AddListener(OnAmbientChanged);
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        // Release handlers
        musicSlider.GetComponent<SliderReleaseHandler>()
            .OnReleased += _ => AudioManager.Instance.PlayMusicOneShot(testSound);

        sfxSlider.GetComponent<SliderReleaseHandler>()
            .OnReleased += _ => AudioManager.Instance.PlaySFX(testSound);

        voiceSlider.GetComponent<SliderReleaseHandler>()
            .OnReleased += _ => AudioManager.Instance.PlayVoiceOneShot(testSound);

        ambientSlider.GetComponent<SliderReleaseHandler>()
            .OnReleased += _ => AudioManager.Instance.PlayAmbientOneShot(testSound);

        // Check which screen
        if (SceneManager.GetActiveScene().name == Constants.mainMenuSceneString)
        {
            resumeButton.interactable = false;
        }
        else
        {
            resumeButton.interactable = true;
        }
    }

    private void OnEnable()
    {
        SettingsIsOpen = true;

        if (EventSystem.current == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
        }
    }

    public void OnDisable()
    {
        GameSettings.Instance.SaveSettings();
        SettingsIsOpen = false;
    }

    public void OnMusicChanged(float value)
    {
        GameSettings.Instance.musicVolume = value;
    }

    public void OnSFXChanged(float value)
    {
        GameSettings.Instance.sfxVolume = value;
    }

    public void OnVoiceChanged(float value)
    {
        GameSettings.Instance.voiceVolume = value;
    }

    public void OnAmbientChanged(float value)
    {
        GameSettings.Instance.ambientVolume = value;
    }

    public void OnSensitivityChanged(float value)
    {
        GameSettings.Instance.mouseSensitivity = value;
    }

    void OnVsyncToggleChanged(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        Debug.Log("Vsync is " + isOn);
        GameSettings.Instance.vsync = isOn;
    }

    void PopulateFullscreenDropdown()
    {
        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.AddOptions(new List<string>
    {
        "Windowed",
        "Borderless",
        "Fullscreen"
    });

        fullscreenDropdown.value = GameSettings.Instance.fullscreenModeIndex;
        fullscreenDropdown.RefreshShownValue();
        fullscreenDropdown.onValueChanged.AddListener(OnFullscreenModeChanged);
    }
    void PopulateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        Resolution[] resolutions = GameSettings.Instance.resolutions;

        // If for some reason the list is empty, fallback
        if (resolutions == null || resolutions.Length == 0)
        {
            resolutions = Screen.resolutions;
        }

        List<string> options = new();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = GameSettings.Instance.resolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    void OnFullscreenModeChanged(int index)
    {
        GameSettings.Instance.fullscreenModeIndex = index;
        GameSettings.Instance.ApplyDisplaySettings();
        GameSettings.Instance.SaveSettings();
    }

    void OnResolutionChanged(int index)
    {
        GameSettings.Instance.resolutionIndex = index;
        GameSettings.Instance.ApplyDisplaySettings();
        GameSettings.Instance.SaveSettings();
    }

    public void ResumeGame()
    {
        SettingsIsOpen = false;
        SceneTransition.Instance.StartTransitionUnload(Constants.settingsSceneString, 0f);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        PlayerController.EnablePlayerControl();
    }

    public void MainMenu()
    {
        if (SceneManager.GetActiveScene().name == Constants.mainMenuSceneString)
        {
            SceneTransition.Instance.StartTransitionUnload(Constants.settingsSceneString, Constants.settingsFadeTime);
        }
        else
        {
            SceneTransition.Instance.StartTransition(Constants.mainMenuSceneString, LoadSceneMode.Single, Constants.settingsFadeTime);
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
            SettingsIsOpen = false;
        }
    }
}
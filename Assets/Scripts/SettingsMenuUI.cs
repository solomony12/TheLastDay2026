using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Sliders")]
    public UnityEngine.UI.Slider musicSlider;
    public UnityEngine.UI.Slider sfxSlider;
    public UnityEngine.UI.Slider voiceSlider;
    public UnityEngine.UI.Slider sensitivitySlider;
    public UnityEngine.UI.Toggle vsyncToggle;

    public UnityEngine.UI.Button resumeButton;
    public UnityEngine.UI.Button mainMenuButton;

    private const string mainMenuSceneString = "MainMenu";
    private const string settingsSceneString = "Settings";

    public static bool SettingsIsOpen = false;

    public AudioClip testSound;

    private void Start()
    {
        // Load saved values into sliders
        musicSlider.value = GameSettings.Instance.musicVolume;
        sfxSlider.value = GameSettings.Instance.sfxVolume;
        voiceSlider.value = GameSettings.Instance.voiceVolume;
        sensitivitySlider.value = GameSettings.Instance.mouseSensitivity;
        vsyncToggle.isOn = GameSettings.Instance.vsync;

        // Toggle
        vsyncToggle.onValueChanged.AddListener(OnVsyncToggleChanged);

        // Add listeners
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        voiceSlider.onValueChanged.AddListener(OnVoiceChanged);
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        // Release handlers
        musicSlider.GetComponent<SliderReleaseHandler>()
            .OnReleased += _ => AudioManager.Instance.PlayMusicOneShot(testSound);

        sfxSlider.GetComponent<SliderReleaseHandler>()
            .OnReleased += _ => AudioManager.Instance.PlaySFX(testSound);

        voiceSlider.GetComponent<SliderReleaseHandler>()
            .OnReleased += _ => AudioManager.Instance.PlayVoiceOneShot(testSound);

        // Check which screen
        if (SceneManager.GetActiveScene().name == mainMenuSceneString)
        {
            resumeButton.interactable = false;
        }
        else
        {
            resumeButton.interactable = true;
        }
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

    public void OnDisable()
    {
        GameSettings.Instance.SaveSettings();
        SettingsIsOpen = false;
    }

    private void OnEnable()
    {
        SettingsIsOpen = true;
    }

    public void ResumeGame()
    {
        SettingsIsOpen = false;
        if (!SceneManager.GetSceneByName(mainMenuSceneString).isLoaded)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            PlayerController.EnablePlayerControl();
        }
        if (SceneManager.GetSceneByName(settingsSceneString).isLoaded)
        {
            SceneManager.UnloadSceneAsync(settingsSceneString);
        }
    }

    public void MainMenu()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        SettingsIsOpen = false;

        if (SceneManager.GetSceneByName(mainMenuSceneString).isLoaded)
        {
            //AudioClip mainMusic = Resources.Load<AudioClip>("Music/main");
            //AudioManager.Instance.PlayMusic(mainMusic, true);
            SceneManager.UnloadSceneAsync(settingsSceneString);
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            AudioClip mainMusic = Resources.Load<AudioClip>("Music/main2");
            AudioManager.Instance.SwitchMusic(mainMusic);
            SceneTransition.Instance.StartTransition(mainMenuSceneString);
        }
    }
}
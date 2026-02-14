using System.Linq;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("Audio")]
    public float musicVolume = 0.9f;
    public float sfxVolume = 0.8f;
    public float voiceVolume = 1f;
    public float ambientVolume = 0.5f;

    [Header("Controls")]
    public float mouseSensitivity = 1f;

    [Header("Other")]
    public bool vsync = true;

    [Header("Display")]
    public int fullscreenModeIndex = 1; // 0 Windowed, 1 Borderless, 2 Fullscreen
    public int resolutionIndex = 0;

    [HideInInspector] public Resolution[] resolutions;

    private PlayerInputHandler inputHandler;

    private float inputDelay = 0.15f;
    private float inputTimer = 0f;

    private bool noOtherScreensAreUp;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        inputTimer -= Time.deltaTime;
        if (inputTimer > 0) return;

        if (SceneManager.GetActiveScene().name == Constants.mainMenuSceneString)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseResume();
            inputTimer = inputDelay;
        }
    }

    private void PauseResume()
    {
        // Resume
        if (SceneManager.GetSceneByName(Constants.settingsSceneString).isLoaded)
        {
            SettingsMenuUI.SettingsIsOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            PlayerController.EnablePlayerControl();
            SceneManager.UnloadSceneAsync(Constants.settingsSceneString);
        }
        // Pause
        else
        {
            SettingsMenuUI.SettingsIsOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerController.DisablePlayerControl();
            SceneManager.LoadScene(Constants.settingsSceneString, LoadSceneMode.Additive);
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("voiceVolume", voiceVolume);
        PlayerPrefs.SetFloat("ambientVolume", ambientVolume);
        PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);

        PlayerPrefs.SetInt("vsyncToggle", vsync ? 1 : 0);

        PlayerPrefs.SetInt("fullscreenMode", fullscreenModeIndex);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);

        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("musicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", sfxVolume);
        voiceVolume = PlayerPrefs.GetFloat("voiceVolume", voiceVolume);
        ambientVolume = PlayerPrefs.GetFloat("ambientVolume", ambientVolume);
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", mouseSensitivity);
        vsync = PlayerPrefs.GetInt("vsyncToggle", 1) == 1;

        fullscreenModeIndex = PlayerPrefs.GetInt("fullscreenMode", fullscreenModeIndex);
        resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", resolutionIndex);

        // Filter resolutions to 16:9 only 
        Resolution[] allResolutions = Screen.resolutions;
        resolutions = allResolutions
            .Where(r => IsSameAspectRatio(r, 16, 9))
            .ToArray();

        ApplyDisplaySettings();
    }

    private bool IsSameAspectRatio(Resolution res, int width, int height)
    {
        // Compare using cross multiplication to avoid floating-point errors
        return res.width * height == res.height * width;
    }

    public void ApplyDisplaySettings()
    {
        // Fullscreen mode
        FullScreenMode mode = fullscreenModeIndex switch
        {
            0 => FullScreenMode.Windowed,
            1 => FullScreenMode.FullScreenWindow,
            2 => FullScreenMode.ExclusiveFullScreen,
            _ => FullScreenMode.FullScreenWindow
        };

        Screen.fullScreenMode = mode;

        // Resolution
        if (resolutions != null && resolutions.Length > 0)
        {
            Resolution res = resolutions[Mathf.Clamp(resolutionIndex, 0, resolutions.Length - 1)];
            Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
        }

        QualitySettings.vSyncCount = vsync ? 1 : 0;
    }

}
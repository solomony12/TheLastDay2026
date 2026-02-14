using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    [Header("Audio")]
    public float musicVolume = 0.3f;
    public float sfxVolume = 1f;
    public float voiceVolume = 1f;

    [Header("Controls")]
    public float mouseSensitivity = 2f;

    [Header("Other")]
    public bool vsync = true;

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

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (inputHandler == null)
                inputHandler = PlayerInputHandler.Instance;

            noOtherScreensAreUp = !SceneTransition.IsTransitioning;
            if (inputHandler.EscapeTriggered && noOtherScreensAreUp)
            {
                PauseResume();
                inputTimer = inputDelay;
            }
        }
    }

    private void PauseResume()
    {
        string settingsSceneString = "Settings";

        // Resume
        if (SceneManager.GetSceneByName(settingsSceneString).isLoaded)
        {
            SettingsMenuUI.SettingsIsOpen = false;
            SceneManager.UnloadSceneAsync(settingsSceneString);
        }
        // Pause
        else
        {
            SettingsMenuUI.SettingsIsOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PlayerController.DisablePlayerControl();
            SceneManager.LoadScene(settingsSceneString, LoadSceneMode.Additive);
        }
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("voiceVolume", voiceVolume);
        PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);

        PlayerPrefs.SetInt("vsyncToggle", vsync ? 1 : 0);

        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("musicVolume", musicVolume);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", sfxVolume);
        voiceVolume = PlayerPrefs.GetFloat("voiceVolume", voiceVolume);
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity", mouseSensitivity);

        vsync = PlayerPrefs.GetInt("vsyncToggle", 1) == 1;
    }
}
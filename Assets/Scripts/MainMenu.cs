using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private const string playSceneName = "1_IntroScene";
    [SerializeField] private const string tutorialSceneName = "TutorialVideo";
    [SerializeField] private const string settingsSceneName = "Settings";

    public Button playButton;

    private static bool tutorialPlayed = false;

    private void Awake()
    {
        playButton.interactable = tutorialPlayed;
    }

    public void Play()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneTransition.Instance.StartOpeningCutscene(playSceneName);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene(tutorialSceneName, LoadSceneMode.Additive);
        tutorialPlayed = true;
        playButton.interactable = tutorialPlayed;
    }

    public void Settings()
    {
        SettingsMenuUI.SettingsIsOpen = true;
        SceneManager.LoadScene(settingsSceneName, LoadSceneMode.Additive);
    }

    public void Credits()
    {
        SceneManager.LoadScene("CreditsPage", LoadSceneMode.Additive);
    }

    public void Quit()
    {
        Debug.Log("Quit button pressed.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
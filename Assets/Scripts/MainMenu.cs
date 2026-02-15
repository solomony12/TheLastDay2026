using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button playButton;

    private static bool tutorialPlayed = false;

    private void Awake()
    {
        playButton.interactable = tutorialPlayed;
    }

    public void Play()
    {
        Cursor.lockState = CursorLockMode.Locked;
        SceneTransition.Instance.StartGameTransition(Constants.mainSceneString);
    }

    public void Tutorial()
    {
        SceneTransition.Instance.StartTransition(Constants.tutorialSceneName, LoadSceneMode.Additive, Constants.settingsFadeTime);
        tutorialPlayed = true;
        playButton.interactable = tutorialPlayed;
    }

    public void Settings()
    {
        SettingsMenuUI.SettingsIsOpen = true;
        SceneTransition.Instance.StartTransition(Constants.settingsSceneString, LoadSceneMode.Additive, Constants.settingsFadeTime);
    }

    public void Credits()
    {
        SceneTransition.Instance.StartTransition(Constants.creditsSceneString, LoadSceneMode.Additive, Constants.settingsFadeTime);
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
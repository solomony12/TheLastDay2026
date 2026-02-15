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
        SceneManager.LoadScene(Constants.tutorialSceneName, LoadSceneMode.Additive);
        tutorialPlayed = true;
        playButton.interactable = tutorialPlayed;
    }

    public void Settings()
    {
        SettingsMenuUI.SettingsIsOpen = true;
        SceneManager.LoadScene(Constants.settingsSceneString, LoadSceneMode.Additive);
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
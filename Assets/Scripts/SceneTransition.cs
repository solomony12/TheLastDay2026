using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    private CanvasGroup fadeCanvasGroup;

    public static SceneTransition Instance { get; private set; }

    public static bool IsTransitioning = false;

    private GameObject uiCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        fadeCanvasGroup = fadeImage.GetComponent<CanvasGroup>();
        if (fadeCanvasGroup == null)
            fadeCanvasGroup = fadeImage.gameObject.AddComponent<CanvasGroup>();

        fadeCanvasGroup.blocksRaycasts = false;
        fadeImage.color = new Color(0, 0, 0, 0);

        //uiCanvas = GameObject.FindWithTag("UI_Canvas"); // TODO: Make tag and canvas
        //uiCanvas.SetActive(false);
    }

    public void StartTransition(string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single, float fadeDuration = 1f)
    {
        StartCoroutine(FadeAndLoad(sceneName, sceneMode, fadeDuration));
    }

    private IEnumerator FadeAndLoad(string sceneName, LoadSceneMode sceneMode, float fadeDuration)
    {
        // Block clicks
        fadeCanvasGroup.blocksRaycasts = true;
        IsTransitioning = true;
        PlayerController.DisablePlayerControl();

        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Load the new scene
        yield return SceneManager.LoadSceneAsync(sceneName, sceneMode);
        PlayerController.DisablePlayerControl();

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Allow clicks again
        fadeCanvasGroup.blocksRaycasts = false;
        IsTransitioning = false;
        PlayerController.EnablePlayerControl();
    }

    /// <summary>
    /// Used to unload the current additive scene
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="fadeDuration"></param>
    public void StartTransitionUnload(string sceneName, float fadeDuration = 1f)
    {
        StartCoroutine(FadeAndUnload(sceneName, fadeDuration));
    }

    private IEnumerator FadeAndUnload(string sceneName, float fadeDuration)
    {
        // Block clicks
        fadeCanvasGroup.blocksRaycasts = true;
        IsTransitioning = true;

        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Unload the additive scene
        yield return SceneManager.UnloadSceneAsync(sceneName);

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Allow clicks again
        fadeCanvasGroup.blocksRaycasts = false;
        IsTransitioning = false;
        PlayerController.EnablePlayerControl();
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float fadeDuration)
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, endAlpha);
    }

    public void StartGameTransition(string sceneName, LoadSceneMode sceneMode = LoadSceneMode.Single, float fadeDuration = 1f)
    {
        StartCoroutine(FadeAndLoadStartGame(sceneName, sceneMode, fadeDuration));
    }

    private IEnumerator FadeAndLoadStartGame(string sceneName, LoadSceneMode sceneMode, float fadeDuration)
    {
        // Block clicks
        fadeCanvasGroup.blocksRaycasts = true;
        IsTransitioning = true;
        PlayerController.DisablePlayerControl();

        HoverCaptions.Instance.HideCaptions();
        Captions.Instance.HideCaptions(0);

        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Load the new scene
        yield return SceneManager.LoadSceneAsync(sceneName, sceneMode);
        PlayerController.DisablePlayerControl();

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // Allow clicks again
        fadeCanvasGroup.blocksRaycasts = false;
        IsTransitioning = false;
        PlayerController.EnablePlayerControl();
    }

}
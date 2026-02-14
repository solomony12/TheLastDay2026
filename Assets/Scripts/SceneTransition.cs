using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private CanvasGroup canvasGroup;

    public static SceneTransition Instance { get; private set; }

    public static bool IsTransitioning = false;

    [Header("GameObjects")]
    private Camera mainCamera;
    private GameObject player;
    private GameObject gun;
    private GameObject canvas;

    private Animator cameraAnimator;

    public static float videoLength = 32f; // TODO: 32f

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

        canvasGroup = fadeImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = fadeImage.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.blocksRaycasts = false;
        fadeImage.color = new Color(0, 0, 0, 0);

        canvas = GameObject.FindWithTag("GameCanvas");
        canvas.SetActive(false);
    }

    public void StartTransition(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // Block clicks
        canvasGroup.blocksRaycasts = true;
        IsTransitioning = true;
        PlayerController.DisablePlayerControl();

        // Fade in
        yield return StartCoroutine(Fade(0f, 1f));

        // Load the new scene
        yield return SceneManager.LoadSceneAsync(sceneName);
        PlayerController.DisablePlayerControl();

        // Fade out
        yield return StartCoroutine(Fade(1f, 0f));

        // Allow clicks again
        canvasGroup.blocksRaycasts = false;
        IsTransitioning = false;
        PlayerController.EnablePlayerControl();
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
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

    public void StartOpeningCutscene(string sceneName)
    {
        StartCoroutine(FadeAndLoadOpening(sceneName));
    }

    private IEnumerator FadeAndLoadOpening(string sceneName)
    {
        // Block clicks
        canvasGroup.blocksRaycasts = true;
        IsTransitioning = true;

        // Fade in
        yield return StartCoroutine(Fade(0f, 2f));

        // Load the new scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Set variables upon scene loaded
        mainCamera = Camera.main;
        player = GameObject.FindWithTag("Player");
        gun = GameObject.FindWithTag("Gun");
        cameraAnimator = mainCamera.GetComponent<Animator>();
        cameraAnimator.ResetTrigger("StartZoomingOut");
        PlayerController.DisablePlayerControl();

        // Set views (animation for camera is already set)
        gun.SetActive(false);
        //try { canvas.SetActive(false); }
        //catch (Exception e) { Debug.Log(e);  }

        // Fade out
        yield return StartCoroutine(Fade(0.5f, 0f));

        // Allow clicks again
        canvasGroup.blocksRaycasts = false;

        float zoomLength = 10f;
        float lengthOfVideoMinusZoom = videoLength - zoomLength;
        yield return new WaitForSeconds(lengthOfVideoMinusZoom);

        // As the video nears the end, we start zooming out
        cameraAnimator.SetTrigger("StartZoomingOut");

        yield return new WaitForSeconds(zoomLength);

        // Start game
        gun.SetActive(true);
        canvas.SetActive(true);
        Captions.Instance.HideCaptions(0f);
        Destroy(cameraAnimator); // this is cause it breaks the camera crouching
        PlayerController.Instance.StartingPositionSet(0.2f);
        yield return new WaitForSeconds(0.2f);
        IsTransitioning = false;
        PlayerController.EnablePlayerControl();
        PlayerController.Instance.ForceStanding();
        Captions.Instance.TimedShowCaptions("Explore the factory\n([W/A/S/D], [Space], [Left Ctrl], [Left Shift])", 10f);
    }
}
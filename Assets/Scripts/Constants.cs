using UnityEngine;

public class Constants : MonoBehaviour
{
    [Header("Scene Names")]
    public static string mainMenuSceneString = "MainMenu";
    public static string settingsSceneString = "Settings";
    public static string videoSceneString = "TutorialVideo";
    public static string creditsSceneString = "CreditsPage";
    public static string mainSceneString = "SampleScene";

    [Header("Tags")]
    public static string playerTag = "Player";
    public static string gunTag = "Gun";
    public static string zombieTag = "Zombie";

    [Header("Paths")]
    public static string sfxPath = "SFX";
    public static string musicPath = "Music";

    [Header("Music Audio Clips")]
    public static AudioClip mainMusic = Resources.Load<AudioClip>($"{musicPath}/MainTheme");

    [Header("Ambient Audio Clips")]
    public static AudioClip hum = Resources.Load<AudioClip>($"{sfxPath}/hum");

    [Header("Time Values")]
    public static float settingsFadeTime = 0.25f;
}
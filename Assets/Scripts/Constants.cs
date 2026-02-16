using UnityEngine;

public class Constants : MonoBehaviour
{
    [Header("Scene Names")]
    public static readonly string mainMenuSceneString = "MainMenu";
    public static readonly string settingsSceneString = "Settings";
    public static readonly string tutorialSceneName = "TutorialVideo";
    public static readonly string creditsSceneString = "CreditsPage";
    public static readonly string mainSceneString = "SampleScene";
    public static readonly string endingCharacterCreditsSceneString = "EndingCharacterCreditsScene";

    [Header("Tags")]
    public static readonly string untaggedTag = "Untagged";
    public static readonly string playerTag = "Player";
    public static readonly string gunTag = "Gun";
    public static readonly string zombieTag = "Zombie";
    public static readonly string curesTextTag = "CuresText";

    [Header("Paths")]
    public static readonly string sfxPath = "SFX";
    public static readonly string musicPath = "Music";
    public static readonly string profilePicturesPath = "Art/ProfilePictures";

    [Header("Music Audio Clips")]
    public static AudioClip mainMusic = Resources.Load<AudioClip>($"{musicPath}/MainTheme");

    [Header("Ambient Audio Clips")]
    public static AudioClip hum = Resources.Load<AudioClip>($"{sfxPath}/hum");

    [Header("Time Values")]
    public static float settingsFadeTime = 0.25f;
}
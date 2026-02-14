using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioSource backgroundHum;

    [Header("Music Transition")]
    [SerializeField] private float musicFadeDuration = 0.5f;

    private Coroutine musicFadeRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Continuously sync volumes from GameSettings
        if (GameSettings.Instance == null) return;

        musicSource.volume = GameSettings.Instance.musicVolume;
        sfxSource.volume = GameSettings.Instance.sfxVolume;
        voiceSource.volume = GameSettings.Instance.voiceVolume;

        // Lowest is 0.1 unless SFX is 0, in which it's also 0
        float sfx = GameSettings.Instance.sfxVolume;
        backgroundHum.volume = sfx == 0f
            ? 0f
            : Mathf.Max(0.1f, sfx - 0.3f);
    }

    private void InitializeSources()
    {
        // Music source
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        // SFX source
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        // Voice source
        if (voiceSource == null)
        {
            voiceSource = gameObject.AddComponent<AudioSource>();
            voiceSource.loop = false;
            voiceSource.playOnAwake = false;
        }

        // Hum source
        if (backgroundHum == null)
        {
            backgroundHum = gameObject.AddComponent<AudioSource>();
            backgroundHum.loop = true;
            backgroundHum.playOnAwake = true;
        }

    }

    // Music

    public void PlayMusic(AudioClip clip, bool loop = true, bool restartIfSame = false)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying && !restartIfSame)
            return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }

    public void PlayMusicOneShot(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null) return;

        musicSource.PlayOneShot(clip, volumeMultiplier);
    }

    public void SwitchMusic(AudioClip newClip, float fadeDuration = -1f)
    {
        if (newClip == null)
            return;

        if (musicSource.clip == newClip)
            return;

        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);

        float duration = fadeDuration > 0 ? fadeDuration : musicFadeDuration;
        musicFadeRoutine = StartCoroutine(FadeMusicRoutine(newClip, duration));
    }

    private System.Collections.IEnumerator FadeMusicRoutine(AudioClip newClip, float duration)
    {
        float startVolume = musicSource.volume;

        // Fade out
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, GameSettings.Instance.musicVolume, t / duration);
            yield return null;
        }

        musicSource.volume = GameSettings.Instance.musicVolume;
        musicFadeRoutine = null;
    }


    // SFX

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumeMultiplier);
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier = 1, float cutout = -1f)
    {
        if (clip == null) return;

        sfxSource.clip = clip;
        sfxSource.volume = sfxSource.volume * volumeMultiplier;
        sfxSource.Play();

        if (cutout > 0f)
            StartCoroutine(StopAfterSeconds(cutout));
    }

    private IEnumerator StopAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        sfxSource.Stop();
    }


    // Voice
    public void PlayVoice(AudioClip clip, bool interrupt = true)
    {
        if (clip == null) return;

        if (voiceSource.isPlaying && !interrupt)
            return;

        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void StopVoice()
    {
        voiceSource.Stop();
        voiceSource.clip = null;
    }

    public void PlayVoiceOneShot(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null) return;

        voiceSource.PlayOneShot(clip, volumeMultiplier);
    }

    public bool IsVoicePlaying()
    {
        return voiceSource.isPlaying;
    }

    // Background Hum
    public void PlayBackgroundHum(AudioClip clip)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        backgroundHum.clip = clip;
        backgroundHum.Play();
    }

    public void StopBackgroundHum()
    {
        backgroundHum.Stop();
        backgroundHum.clip = null;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Scene Loaded: " + scene.name);

        if (SceneManager.GetSceneByName("TutorialVideo").isLoaded)
        {
            StopMusic();
        }

        if (mode == LoadSceneMode.Additive)
            return;

        StopBackgroundHum();

        switch (scene.name)
        {
            case "MainMenu":
                //Debug.Log("MainMenu");
                AudioClip fasterMusic = Resources.Load<AudioClip>("Music/main2");
                SwitchMusic(fasterMusic, 0.1f);
                break;
            case "1_IntroScene":
                //Debug.Log("Intro Scene Loaded");
                StopMusic();
                StartCoroutine(WaitForIntro());
                break;

            case "2_Warehouse_Scene":
                //Debug.Log("Warehouse Loaded");

                // Office music for the warehouse
                AudioClip officeMusic = Resources.Load<AudioClip>("Music/office");
                SwitchMusic(officeMusic);


                AudioClip hum = Resources.Load<AudioClip>("Sounds/loud-machinery-449526");
                PlayBackgroundHum(hum);
                break;

            case "3_FactoryFloor":
                //Debug.Log("Factory Loaded");

                // Warehouse music for factory
                AudioClip warehouseMusic = Resources.Load<AudioClip>("Music/warehouse");
                SwitchMusic(warehouseMusic);

                hum = Resources.Load<AudioClip>("Sounds/loud-machinery-449526");
                PlayBackgroundHum(hum);
                break;

            case "4_Office":
                //Debug.Log("Office Loaded");

                // Factory music carries over to Office before blackout ends it

                AudioClip officeHum = Resources.Load<AudioClip>("Sounds/low-engine-hum-72529_LV4");
                PlayBackgroundHum(officeHum);
                break;

            case "6_FinalArea":
                //Debug.Log("Final Area Loaded");
                // Do Final Area stuff here
                StopMusic();
                break;

            case "7_EndingLevel":
                //Debug.Log("Ending Level Loaded");
                StopMusic();
                AudioClip heartbeat = Resources.Load<AudioClip>("Sounds/heartbeat-sound-372448_LV7");
                PlayBackgroundHum(heartbeat);
                break;

            case "TimedGameOverScene":
                AudioClip staticHum = Resources.Load<AudioClip>("Sounds/tv-static-323620");
                PlayBackgroundHum(staticHum);
                break;

            default:
                Debug.Log("Scene not in list");
                break;
        }
    }

    private IEnumerator WaitForIntro()
    {
        float waitTime = SceneTransition.videoLength - 2;
        yield return new WaitForSeconds(waitTime);
        AudioClip fasterMusic = Resources.Load<AudioClip>("Music/main2");
        PlayMusic(fasterMusic);
    }
}
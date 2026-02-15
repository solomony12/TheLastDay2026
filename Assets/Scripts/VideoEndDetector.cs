using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoEndDetector : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        EndVideo();
    }

    public void SkipVideo()
    {
        EndVideo();
    }

    private void EndVideo()
    {
        AudioClip mainMusic = Resources.Load<AudioClip>("Music/main2");
        AudioManager.Instance.PlayMusic(mainMusic, true);

        SceneTransition.Instance.StartTransitionUnload(Constants.tutorialSceneName, Constants.settingsFadeTime);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsPageScript : MonoBehaviour
{
    public void Return()
    {
        SceneTransition.Instance.StartTransitionUnload(Constants.creditsSceneString, Constants.settingsFadeTime);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsPageScript : MonoBehaviour
{
    public void Return()
    {
        SceneManager.UnloadSceneAsync("CreditsPage");
    }
}
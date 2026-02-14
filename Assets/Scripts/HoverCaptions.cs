using UnityEngine;
using TMPro;

public class HoverCaptions : MonoBehaviour
{
    private TMP_Text captions;

    public static HoverCaptions Instance { get; private set; }

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

        captions = GameObject.FindWithTag("HoverCaptions").GetComponent<TMP_Text>();
    }

    public void ShowCaptions(string text)
    {
        captions.text = text;
        captions.enabled = true;
    }
    public void HideCaptions()
    {
        captions.enabled = false;
    }
}
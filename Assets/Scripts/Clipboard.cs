using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Clipboard : MonoBehaviour
{
    public InputAction toggleClipboardAction;
    public InputAction nextPatientAction;
    public InputAction previousPatientAction;

    private static bool isClipboardUp = false;
    private static int currentPatient = 0;

    private string[] names;
    private Dictionary<string, string> descriptionDict
        = new Dictionary<string, string>
        {
            { "TEST_ZOMBIE", "- Tall\n- Stupid\n- Dumb"},
        };

    private TMP_Text characterName;
    private TMP_Text description;
    private Image image;

    private void OnEnable()
    {
        toggleClipboardAction.Enable();
        nextPatientAction.Enable();
        previousPatientAction.Enable();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        toggleClipboardAction.Disable();
        nextPatientAction.Disable();
        previousPatientAction.Disable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Constants.clipboardSceneString)
        {
            SetGameObjects();
        }
    }

    private void Awake()
    {
        var zombies = CureSystem.Instance.zombies;
        names = new string[zombies.Length];
        for (int i = 0; i < zombies.Length; i++)
        {
            names[0] = zombies[i].name;
        }

        toggleClipboardAction.performed += ToggleClipboard;
        nextPatientAction.performed += ChangePatientInfoNext;
        previousPatientAction.performed += ChangePatientInfoPrev;
    }

    private void ToggleClipboard(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isClipboardUp)
            {
                SceneTransition.Instance.StartTransitionUnload(Constants.clipboardSceneString, 0f);
                isClipboardUp = false;
            }
            else
            {
                SceneTransition.Instance.StartTransition(Constants.clipboardSceneString, UnityEngine.SceneManagement.LoadSceneMode.Additive, 0f);
                isClipboardUp = true;
            }
        }
    }

    private void SetGameObjects()
    {
        // TODO: true gameobject names
        characterName = GameObject.Find("Name").GetComponent<TMP_Text>();
        description = GameObject.Find("Description").GetComponent<TMP_Text>();
        image = GameObject.Find("Image").GetComponent<Image>();
    }

    private void ChangePatientInfoNext(InputAction.CallbackContext context)
    {
        if (context.performed && isClipboardUp)
        {
            UpdateInfo(1);
        }
    }

    private void ChangePatientInfoPrev(InputAction.CallbackContext context)
    {
        if (context.performed && isClipboardUp)
        {
            UpdateInfo(-1);
        }
    }

    private void UpdateInfo(int nextOrPrev)
    {
        int length = names.Length - 1;
        currentPatient += nextOrPrev;
        if (currentPatient < 0)
            currentPatient = length;
        else if (currentPatient >= length)
            currentPatient = 0;

        // TODO: Update info
        string currentName = names[currentPatient];
        string currentDescription = descriptionDict[currentName];

        characterName.text = currentName;
        description.text = currentDescription;
        // TODO: load image
        image.sprite = Resources.Load<Sprite>($"{Constants.profilePicturesPath}/{currentName}");
    }

}

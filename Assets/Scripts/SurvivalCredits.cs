using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class SurvivalCredits : MonoBehaviour
{
    public static SurvivalCredits Instance;

    private List<SurviveZombie> charactersList;
    private readonly Dictionary<SurviveZombie, string> zombiesEndingTextDict
        = new Dictionary<SurviveZombie, string>()
        {
            { new SurviveZombie("Kid", true), "Was a known troublemaker." },
            { new SurviveZombie("Kid", false), "Only child of a poor family." },
            { new SurviveZombie("TEST_ZOMBIE", true), "Cured zombie." },
            { new SurviveZombie("TEST_ZOMBIE", false), "Gunned-down zombie." }
        };
    private Dictionary<string, string> nameToConclusionDict;

    [Header("Unity Canvas Objects")]
    private Image characterPicture;
    private TMP_Text characterName;
    private TMP_Text characterEnding;
    private GameObject continueText;

    private IEnumerator<KeyValuePair<string, string>> _enumerator;

    private Coroutine textCoroutine;
    private readonly float typingSpeed = 0.05f;
    private bool characterTextDoneLoading = false;

    public InputAction clickAction;

    private void OnEnable() => clickAction.Enable();
    private void OnDisable() => clickAction.Disable();

    private void Awake()
    {
        GetCanvasObjects();
        BuildSurvivorsList();
        BuildEndingCharacterCredits();

        clickAction.performed += OnClick;

        characterTextDoneLoading = true;
        continueText.SetActive(true);
    }

    public void GetCanvasObjects()
    {
        characterPicture = GameObject.Find("CharacterProfilePicture").GetComponent<Image>();
        characterName = GameObject.Find("NameText").GetComponent<TMP_Text>();
        characterEnding = GameObject.Find("ResultText").GetComponent<TMP_Text>();
        continueText = GameObject.Find("ContinueText");

        if (characterPicture == null
            || characterName == null
            || characterEnding == null
            || continueText == null)
        {
            Debug.LogError("One or more canvas objects not found");
        }
    }

    public void BuildSurvivorsList()
    {
        Dictionary<string, bool> list = CureSystem.zombiesCuredDict;

        charactersList = new List<SurviveZombie>();

        SurviveZombie surviveZombie;
        foreach (var character in list)
        {
            Debug.Log($"{character.Key}, {character.Value}");
            surviveZombie = new SurviveZombie(character.Key, character.Value);
            charactersList.Add(surviveZombie);
        }
    }

    public void BuildEndingCharacterCredits()
    {
        nameToConclusionDict = new Dictionary<string, string>();

        foreach (var character in charactersList)
        {
            // Find the matching zombie in zombiesEndingTextDict
            string text = null;

            foreach (var kvp in zombiesEndingTextDict)
            {
                if (kvp.Key.name == character.name && kvp.Key.cured == character.cured)
                {
                    text = kvp.Value;
                    break;
                }
            }

            if (text != null)
            {
                nameToConclusionDict.Add(character.name, text);
            }
            else
            {
                Debug.LogWarning($"No ending text found for {character.name} (cured={character.cured})");
            }
        }

        // Create enumerator for later use
        _enumerator = nameToConclusionDict.GetEnumerator();
    }

    public KeyValuePair<string, string>? GetNextEndingCharacter()
    {
        if (_enumerator.MoveNext())
        {
            return _enumerator.Current;
        }

        return null; // no more entries
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (characterTextDoneLoading)
            {
                characterTextDoneLoading = false;
                continueText.SetActive(false);
                var pair = GetNextEndingCharacter();

                if (pair != null)
                {
                    UpdateScreen(pair.Value.Key, pair.Value.Value);
                }
                else
                {
                    SceneTransition.Instance.StartTransition(Constants.mainMenuSceneString);
                }
            }
        }
    }

    public void UpdateScreen(string name, string text)
    {
        characterName.text = name;
        characterPicture.sprite = Resources.Load<Sprite>($"{Constants.profilePicturesPath}/{name}");
        StartTyping(text);
    }

    public void StartTyping(string fullText)
    {
        if (textCoroutine != null)
            StopCoroutine(textCoroutine);
        textCoroutine = StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string fullText)
    {
        characterEnding.text = "";

        foreach (char letter in fullText)
        {
            characterEnding.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        characterTextDoneLoading = true;
        continueText.SetActive(true);
    }
}

public class SurviveZombie
{
    public string name;
    public bool cured;
    //public string endingText;

    public SurviveZombie(string n, bool c)
    {
        name = n;
        cured = c;

        //string survive = c ? "survived" : "died as a zombie";
        //endingText = $"{name} {survive}.";
    }
}
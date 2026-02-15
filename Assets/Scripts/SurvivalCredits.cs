using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SurvivalCredits : MonoBehaviour
{
    public static SurvivalCredits Instance;

    private List<SurviveZombie> charactersList;
    private readonly Dictionary<SurviveZombie, string> zombiesEndingTextDict
        = new Dictionary<SurviveZombie, string>()
        {
            { new SurviveZombie("Kid", true), "Was a known troublemaker." },
            { new SurviveZombie("Kid", false), "Only child of a poor family." }
        };
    private Dictionary<string, string> nameToConclusionDict;

    [Header("Unity Canvas Objects")]
    private Image characterPicture;
    private TMP_Text characterName;
    private TMP_Text characterEnding;

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
    }

    public void GetCanvasObjects()
    {
        characterPicture = GameObject.Find("").GetComponent<Image>();
        characterName = GameObject.Find("").GetComponent<TMP_Text>();
        characterEnding = GameObject.Find("").GetComponent<TMP_Text>();

        if (characterPicture == null
            || characterName == null
            || characterEnding == null)
        {
            Debug.LogError("One or more canvas objects not found");
        }
    }

    public void BuildSurvivorsList(Dictionary<string, bool> list)
    {
        charactersList = new List<SurviveZombie>();

        SurviveZombie surviveZombie;
        foreach (var character in list)
        {
            surviveZombie = new SurviveZombie(character.Key, character.Value);
            charactersList.Add(surviveZombie);
        }
    }

    public void BuildEndingCharacterCredits()
    {
        nameToConclusionDict = new Dictionary<string, string>();

        string text;
        foreach (var character in charactersList)
        {
            text = zombiesEndingTextDict[character];
            nameToConclusionDict.Add(character.name, text);
        }
    }

    public string GetNextEndingCharacter()
    {
        IEnumerator enumerator = nameToConclusionDict.GetEnumerator();
        yield return enumerator.MoveNext;
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
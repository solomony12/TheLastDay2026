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
            { new SurviveZombie("TEST_ZOMBIE", true),
                "CURED: The cure restored more than their pulse — it restored their personality. Records quickly confirmed a history of fraud, assault charges that never stuck, and a pattern of exploiting anyone who trusted them. Within days of recovery, they were already manipulating other survivors for extra rations, proving the infection hadn’t been the worst thing about them." },

            { new SurviveZombie("TEST_ZOMBIE", false),
                "UNCURED: When authorities searched the body, they found a wallet filled with photos of two children and a receipt for supplies purchased for elderly neighbors. Friends later shared that they had been coordinating grocery runs for families too afraid to leave their homes when the first cases appeared." },

            { new SurviveZombie("Baker", true),
                "CURED: Behind the warm smile and flour-dusted apron was a man drowning in gambling debt. The bakery had been weeks from foreclosure, and several small investors later admitted he had falsified accounts to keep borrowing money. After being cured, he immediately began pressuring former customers for loans he could never repay." },

            { new SurviveZombie("Baker", false),
                "UNCURED: Before the outbreak, he quietly delivered free bread to struggling families and kept a list of unpaid tabs he never intended to collect. He sponsored school events and once rebuilt a neighbor’s oven without charging a cent. The community only realized how much he carried them after he was gone." },

            { new SurviveZombie("Boy", true),
                "CURED: Teachers described him as more than mischievous — he had already shown a streak of cruelty, targeting smaller children and lying without remorse. After the cure, he showed little gratitude, bragging that adults always choose to save kids first because they’re easy to feel sorry for." },

            { new SurviveZombie("Boy", false),
                "UNCURED: He used to draw pictures for classmates who were sad and told everyone he would grow up to rescue people in danger. He shared his lunch with a friend who often forgot theirs and once stood up to a bully despite shaking the whole time." },

            { new SurviveZombie("BusinessMan", true),
                "CURED: The cure preserved a man already under investigation for insider trading and illegal labor practices. Former employees came forward describing unpaid overtime and intimidation. Within weeks of recovery, he began consolidating remaining resources for personal security, showing little concern for the workers who had lost everything." },

            { new SurviveZombie("BusinessMan", false),
                "UNCURED: Financial records later showed he had liquidated large portions of his own assets to keep employees paid during early shutdowns. He funded emergency shelters anonymously and ensured health insurance continued for laid-off staff. Few knew how much of his wealth had already been spent protecting others." },

            { new SurviveZombie("Cat", true),
                "CURED: Once cured, the animal proved dangerously aggressive, attacking handlers repeatedly and spreading panic in crowded shelters. Reports suggested it had previously been surrendered by two owners for violent behavior. Keeping it alive required constant sedation and resources already in short supply." },

            { new SurviveZombie("Cat", false),
                "UNCURED: The collar tag led to an elderly owner who relied on the cat for daily companionship. Neighbors said it waited faithfully by the window each afternoon and slept curled at the foot of the bed every night. It had only recently been adopted from a rescue center." },

            { new SurviveZombie("MilitarySoldier", true),
                "CURED: Military records revealed multiple disciplinary investigations for excessive force during civilian operations overseas. Fellow soldiers described him as reckless and eager for confrontation. After being cured, he openly stated that strict control — not compassion — was the only way to rebuild society." },

            { new SurviveZombie("MilitarySoldier", false),
                "UNCURED: Witnesses confirmed he held the evacuation line long enough for dozens of families to escape. He carried two injured strangers to safety before collapsing himself. Letters found in his pack showed regular donations to veterans’ support groups." },

            { new SurviveZombie("Nun", true),
                "CURED: Former members of her parish came forward with stories of coercion and emotional manipulation disguised as spiritual guidance. Donations meant for charity had quietly funded personal comforts. After the cure, she insisted the outbreak was divine punishment and showed little empathy for the grieving." },

            { new SurviveZombie("Nun", false),
                "UNCURED:She operated a soup kitchen that never turned anyone away and routinely gave up her own meals so others could eat. She visited hospitals daily and organized clothing drives every winter. Survivors recall her staying behind to guide children out when panic erupted." },

            { new SurviveZombie("Nurse", true),
                "CURED: Hospital staff later disclosed repeated complaints about negligence and diverted medication supplies. She had been under quiet review before the outbreak began. After recovery, she demanded authority in medical camps despite past mistakes that had already cost lives." },

            { new SurviveZombie("Nurse", false),
                "UNCURED: She worked double shifts during the first wave of chaos, refusing to abandon patients even as infection spread through the ward. Coworkers said she often stayed past exhaustion, comforting strangers when their families could not be present." },

            { new SurviveZombie("OldMan", true),
                "CURED: Neighbors admitted he had a long history of harassment and volatile outbursts, driving away much of his remaining family years ago. After the cure, he continued lashing out at volunteers, insisting younger survivors owed him their supplies out of respect for his age." },

            { new SurviveZombie("OldMan", false),
                "UNCURED: A decorated veteran, he spent years tutoring neighborhood children and volunteering at memorial events. He insisted younger families evacuate first when the outbreak began, refusing assistance until others were safe." },

            { new SurviveZombie("Prisoner", true),
                "CURED: Court documents confirmed the armed robbery conviction was only part of a longer pattern of escalating violence. Several witnesses had been too afraid to testify in earlier cases. Within days of being cured, he was caught intimidating weaker survivors for protection rations." },

            { new SurviveZombie("Prisoner", false),
                "UNCURED: Correctional officers reported he had avoided violence for years and mentored younger inmates trying to earn their GEDs. He sent most of his prison wages to support a younger sibling and was months away from a parole hearing." },

            { new SurviveZombie("Punk", true),
                "CURED: Former classmates described a pattern of vandalism, theft, and organizing destructive protests that left small businesses damaged. After being cured, they openly mocked relief efforts and encouraged others to hoard supplies rather than cooperate." },

            { new SurviveZombie("Punk", false),
                "UNCURED: Despite the loud exterior, they organized small benefit concerts for struggling venues and quietly helped friends cover rent. Teachers once praised their writing for its empathy toward outsiders and the overlooked." }
        };
    private Dictionary<string, string> nameToConclusionDict;

    [Header("Unity Canvas Objects")]
    private Image characterPicture;
    private TMP_Text characterName;
    private TMP_Text characterEnding;
    private GameObject continueText;

    private IEnumerator<KeyValuePair<string, string>> _enumerator;

    private Coroutine textCoroutine;
    private readonly float typingSpeed = 0.01f;
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
        Sprite image = Resources.Load<Sprite>($"{Constants.profilePicturesPath}/{name}_Screenshot");
        if (image == null)
        {
            Debug.LogError("Picture not found");
        }
        characterPicture.sprite = image;
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
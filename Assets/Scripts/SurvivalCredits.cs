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
                "UNCURED: Despite the loud exterior, they organized small benefit concerts for struggling venues and quietly helped friends cover rent. Teachers once praised their writing for its empathy toward outsiders and the overlooked." },

            { new SurviveZombie("Girl", true),
                "CURED: Once the cure took effect, the little girl’s curiosity and innocence twisted into mischief. She began sneaking into supply caches and hoarding toys and treats for herself, showing little concern for other survivors. Her playful nature now carried a streak of cunning and selfishness." },

            { new SurviveZombie("Girl", false),
                "UNCURED: Even as a zombie, she retained glimpses of her sweet, curious nature. She wandered the settlement with wide-eyed wonder, offering small toys she found to other survivors and laughing softly, reminding everyone of the child she once was." }
        };
    /*private readonly Dictionary<SurviveZombie, string> zombiesEndingTextDict
    = new Dictionary<SurviveZombie, string>()
    {
        { new SurviveZombie("Baker", true),
            "CURED: Julia’s bread-making skills and public charisma have now overtaken the community. The survivors devote most of their time to baking and worshiping bread. Agricultural production flourishes, but the obsession distracts from other critical survival tasks." },

        { new SurviveZombie("Baker", false),
            "UNCURED: Without Julia’s guidance, the community struggles to organize itself. While she remains focused on bread, other needs are neglected, leaving survivors to fend for themselves with inconsistent resources." },

        { new SurviveZombie("MilitarySoldier", true),
            "CURED: John continues to enforce order with stoicism and strict rules. His leadership keeps the shelter functioning, though many survivors find his approach harsh. Tasks are completed efficiently, but morale suffers." },

        { new SurviveZombie("MilitarySoldier", false),
            "UNCURED: The shelter is short-staffed and unorganized. Without John’s discipline, tasks pile up, and the survivors struggle to maintain basic operations. Productivity declines as chaos spreads." },

        { new SurviveZombie("Nun", true),
            "CURED: Lauren reestablishes her mission quickly, using survivors’ resources for her own benefit. Donations and supplies are funneled into her schemes, leaving many feeling betrayed and distrustful of her leadership." },

        { new SurviveZombie("Nun", false),
            "UNCURED: Lauren continues her charitable work, running a local mission and helping orphans. She gives up her own needs for others and stays behind during crises, earning the trust and admiration of the survivors." },

        { new SurviveZombie("Wife", true),
            "CURED: Emily recovers physically, but her relationship with you collapses. Soon after the cure, she leaves you for another, leaving you isolated and demoralized despite her health being restored." },

        { new SurviveZombie("Wife", false),
            "UNCURED: Emily survives as a zombie, and while she is no longer herself, you mourn the person she once was. Her absence and transformation weigh heavily on your conscience." },

        { new SurviveZombie("Cat", true),
            "CURED: Snowball escapes to Argentina and quickly establishes dominance over the survivors, turning them into a fascist-style hierarchy with herself at the top. Questioning her authority results in public punishment, making her both feared and adored." },

        { new SurviveZombie("Cat", false),
            "UNCURED: Snowball remains a domestic companion, providing comfort to survivors. She is mischievous but harmless, and her presence boosts morale around the shelter." },

        { new SurviveZombie("Mechanic", true),
            "CURED: Gerald successfully upgrades the factory for faster cure production, but safety protocols are ignored. A catastrophic fire kills six child laborers and halts production for months, leaving the community in disarray." },

        { new SurviveZombie("Mechanic", false),
            "UNCURED: Gerald continues to make small repairs and improvements but lacks the skill to fully optimize production. The factory runs slowly, and minor accidents occur, though no major tragedies happen." },

        { new SurviveZombie("SanitationWorker", true),
            "CURED: Lucy works tirelessly to overhaul the settlement’s sanitation system. Her dedication prevents disease outbreaks and improves hygiene standards, though the effort takes a heavy toll on her own health." },

        { new SurviveZombie("SanitationWorker", false),
            "UNCURED: Lucy succumbs to cholera due to the outbreak, and the settlement lacks sufficient medical supplies to save her. Her sacrifice is remembered, but the community suffers without her guidance." },

        { new SurviveZombie("Retired", true),
            "CURED: Emily’s body is fully restored, but her mind never fully recovers. She stares blankly out the window for hours, unresponsive, leaving others to wonder if her life is now more of a suffering than a blessing." },

        { new SurviveZombie("Retired", false),
            "UNCURED: Emily remains a zombie, but her cheerful nature is lost. Survivors mourn her absence, recognizing the positive impact she had on the community before turning." },

        { new SurviveZombie("Rockstar", true),
            "CURED: Jeanete becomes withdrawn after learning of her girlfriend Lizzie’s death. Though physically cured, she isolates herself from survivors, mourning quietly until one day she ultimately succumbs to grief and joins Lizzie in death." },

        { new SurviveZombie("Rockstar", false),
            "UNCURED: Jeanete continues performing and leading her band, keeping the community entertained and connected despite the outbreak. Her presence inspires hope and unity among survivors." }
    };*/
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

    private AudioClip gunshots;

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

        gunshots = Resources.Load<AudioClip>($"{Constants.sfxPath}/gunshots");
    }

    private void Start()
    {
        AudioManager.Instance.PlaySFX(gunshots);

        if (CureSystem.Instance.currentHealthStatus != CureSystem.HealthStatus.Zombie)
        {
            characterEnding.text = "After all the survivors were gathered, the remaining zombies were gunned down without a second thought in front of you.\r\nThe world soon returned to normal but you'll never erase those memories from your cure delivery days as a child.\r\nYou decided to search up each person to see their past or how they were doing.";
        }
        else
        {
            characterEnding.text = "Your life ended too early as rather than a savior, you became one of them.\r\nWith the remaining cures lost on you, more zombies were gunned down than needed to had you remained alive.\r\nThose you did save... well, let's see what happened to them all.";
        }
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

        // Add the player as the last character
        string playerEnding = null;
        if (CureSystem.Instance.currentHealthStatus != CureSystem.HealthStatus.Healthy && CureSystem.Instance.currentHealthStatus != CureSystem.HealthStatus.Zombie)
        {
            playerEnding = "As for you… well, you became a zombie.\r\nBy the time the infection took hold, it was so late that neither you nor anyone else realized what had happened.\r\nBefore long, the transformation was complete, and you had unleashed another zombie apocalypse.";
        }
        if (playerEnding != null)
        {
            nameToConclusionDict.Add("You (Delivery Boy)", playerEnding);
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
            characterPicture.sprite = null;
        }
        else
        {
            characterPicture.sprite = image;
        }
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
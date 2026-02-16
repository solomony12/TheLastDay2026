using System.Collections.Generic;
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

    /*private Dictionary<string, string> descriptionDict
        = new Dictionary<string, string>
        {
            { "TEST_ZOMBIE", "- Tall\n- Slow moving\n- Blank expression\n- Torn clothes\n- Groans constantly\n- One shoe missing\n- Cloudy eyes\n- Reaches out aimlessly\n- Smells of decay\n- Used to be someone" },

            { "Baker", "- Makes bread every morning\n- Immigrant from Ireland\n- Has a wife and kids\n- Wakes up at 4 a.m.\n- Flour always on his sleeves\n- Knows everyone in town\n- Struggling with debt\n- Gives day-old bread to the poor\n- Smells like yeast\n- Hands rough from work" },

            { "Boy", "- 8 years old\n- Loves drawing superheroes\n- Afraid of the dark\n- Missing his front tooth\n- Wears a red backpack\n- Cries when overwhelmed\n- Wants to be a firefighter\n- Trusts adults easily\n- Scraped knees\n- Carries a stuffed animal" },

            { "BusinessMan", "- Owns multiple companies\n- Always on his phone\n- Drives a luxury car\n- Speaks confidently\n- Rarely home\n- Donates to charity publicly\n- Competitive by nature\n- Expensive tailored suits\n- Has political connections\n- Measures time in money" },

            { "Cat", "- Orange tabby\n- Friendly but skittish\n- Catches mice\n- Purrs loudly\n- Sleeps most of the day\n- Missing part of one ear\n- Rubs against strangers\n- Independent\n- Soft fur\n- Doesn’t understand what’s happening" },

            { "MilitarySoldier", "- Decorated veteran\n- Follows orders strictly\n- Has seen combat\n- Wakes from nightmares\n- Keeps boots polished\n- Protective of civilians\n- Scar across cheek\n- Writes letters home\n- Physically fit\n- Trained to survive" },

            { "Nun", "- Devoutly religious\n- Volunteers at shelters\n- Took a vow of poverty\n- Soft-spoken\n- Wears a simple habit\n- Spends hours in prayer\n- Runs a soup kitchen\n- Forgives easily\n- Believes suffering has meaning\n- Owns very little" },

            { "Nurse", "- Works long hospital shifts\n- Calm under pressure\n- Caring toward patients\n- Drinks too much coffee\n- Missed family holidays\n- Knows how to triage injuries\n- Gentle voice\n- Wears worn-out sneakers\n- Holds hands during final moments\n- Exhausted but keeps going" },

            { "OldMan", "- 87 years old\n- War veteran\n- Walks with a cane\n- Tells long stories\n- Lives alone\n- Keeps faded photographs\n- Hard of hearing\n- Watches the news daily\n- Feels forgotten\n- Remembers another time" },

            { "Prisoner", "- Serving time for armed robbery\n- Claims he’s innocent\n- Covered in tattoos\n- Grew up in poverty\n- Quick temper\n- Protective of younger inmates\n- Reads philosophy books\n- Distrusts authority\n- Strong build\n- Says he wants a second chance" },

            { "Punk", "- Loud and rebellious\n- Plays in a garage band\n- Bright dyed hair\n- Piercings and ripped clothes\n- Questions everything\n- Sleeps on friends’ couches\n- Writes angry lyrics\n- Laughs at danger\n- Hates being judged\n- Secretly cares deeply" }
        };*/
    private Dictionary<string, string> descriptionDict
    = new Dictionary<string, string>
    {
        { "Baker", "- Makes bread every morning\n- Floofy white hat\n- Very talented public speaker\n- Narcissistic\n- Loves bread (maybe a little too much)\n- Converted community to a bread-focused cult\n- Agricultural production flourishes\n- Survivors devote most of their time to bread\n- Hands rough from work\n- Knows everyone in town" },

        { "Boy", "- 8 years old\n- Loves drawing superheroes\n- Afraid of the dark\n- Missing his front tooth\n- Wears a red backpack\n- Cries when overwhelmed\n- Wants to be a firefighter\n- Trusts adults easily\n- Scraped knees\n- Carries a stuffed animal" },

        { "BusinessMan", "- Samuel\n- Quite, brooding\n- Short hair and a tie\n- Wealthy, generous\n- Good at managing people\n- Doesn’t handle stress well\n- Tremendous initial productivity under leadership\n- Later has a breakdown and is moved to psychiatric ward\n- Community productivity grinds to a halt\n- Observant and meticulous" },

        { "Cat", "- Snowball the Cat\n- Cute, adorable\n- Escaped to Argentina after committing various war crimes\n- Turns survivors into a fascist-style dictatorship\n- Supreme leader of the colony\n- Public punishment for questioning her\n- Feared and adored\n- Independent\n- Purrs loudly\n- Rubs against survivors" },

        { "MilitarySoldier", "- John\n- Stoic, harsh, silent\n- Enforces order strictly\n- Shelter always short-staffed\n- Tasks pile up without him\n- Efficient but harsh\n- Protective of survivors\n- Trained to survive\n- Physically fit\n- Scar across cheek" },

        { "Nun", "- Lauren\n- Kind, generous, thoughtful\n- Runs local charity for poor orphans\n- New to town\n- Designated herself to collect tithe offerings\n- Less trustworthy than she seems\n- Milks survivors dry for donations\n- Skipped town overnight\n- Soft-spoken\n- Wears simple habit" },

        { "Nurse", "- Works long hospital shifts\n- Calm under pressure\n- Caring toward patients\n- Drinks too much coffee\n- Missed family holidays\n- Knows how to triage injuries\n- Gentle voice\n- Wears worn-out sneakers\n- Holds hands during final moments\n- Exhausted but keeps going" },

        { "OldMan", "- Randy\n- Old civilian man\n- Laments loss of wife to the hordes\n- Withdrawn and quiet\n- Everyday more isolated\n- Eventually decides to join his wife\n- Hard of hearing\n- Remembers another time\n- Keeps faded photographs\n- Lives alone" },

        { "Prisoner", "- Serving time for armed robbery\n- Claims he’s innocent\n- Covered in tattoos\n- Grew up in poverty\n- Quick temper\n- Protective of younger inmates\n- Reads philosophy books\n- Distrusts authority\n- Strong build\n- Says he wants a second chance" },

        { "Punk", "- Loud and rebellious\n- Plays in a garage band\n- Bright dyed hair\n- Piercings and ripped clothes\n- Questions everything\n- Sleeps on friends’ couches\n- Writes angry lyrics\n- Laughs at danger\n- Hates being judged\n- Secretly cares deeply" },

        { "Girl", "- Young girl\n- Wears a pink dress\n- Innocent and curious\n- Carries a small stuffed animal\n- Wide, bright eyes\n- Tends to wander\n- Easily frightened by loud noises\n- Laughs freely\n- Loves drawing and playing\n- Trusts adults easily\n- Often skips instead of walking" }
    };

    private TMP_Text characterName;
    private TMP_Text description;
    private Image image;

    private void Awake()
    {
        var zombies = CureSystem.Instance.zombies;

        names = new string[zombies.Length];

        for (int i = 0; i < zombies.Length; i++)
        {
            names[i] = zombies[i].name;
        }

        toggleClipboardAction.performed += ToggleClipboard;
        nextPatientAction.performed += ChangePatientInfoNext;
        previousPatientAction.performed += ChangePatientInfoPrev;
    }

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
            UpdateInfo(0);
        }
    }

    private void ToggleClipboard(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (isClipboardUp)
        {
            SceneTransition.Instance.StartTransitionUnload(Constants.clipboardSceneString, 0f);
            isClipboardUp = false;
        }
        else
        {
            SceneTransition.Instance.StartTransition(
                Constants.clipboardSceneString,
                LoadSceneMode.Additive,
                0f
            );
            isClipboardUp = true;
        }
    }

    private void SetGameObjects()
    {
        characterName = GameObject.Find("Name").GetComponent<TMP_Text>();
        description = GameObject.Find("Description").GetComponent<TMP_Text>();
        image = GameObject.Find("Image").GetComponent<Image>();
    }

    private void ChangePatientInfoNext(InputAction.CallbackContext context)
    {
        if (context.performed && isClipboardUp)
            UpdateInfo(1);
    }

    private void ChangePatientInfoPrev(InputAction.CallbackContext context)
    {
        if (context.performed && isClipboardUp)
            UpdateInfo(-1);
    }

    private void UpdateInfo(int direction)
    {
        if (names == null || names.Length == 0)
            return;

        currentPatient += direction;

        if (currentPatient < 0)
            currentPatient = names.Length - 1;
        else if (currentPatient >= names.Length)
            currentPatient = 0;

        string currentName = names[currentPatient];

        characterName.text = currentName;

        if (descriptionDict.TryGetValue(currentName, out string currentDescription))
            description.text = currentDescription;
        else
            description.text = "No description available.";

        Sprite loadedSprite = Resources.Load<Sprite>(
            $"{Constants.profilePicturesPath}/{currentName}_Screenshot"
        );

        if (loadedSprite != null)
            image.sprite = loadedSprite;
    }
}

using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class CureSystem : MonoBehaviour
{
    public static CureSystem Instance;

    private readonly int maxCures = 1;

    private int cureAmounts;

    public static Dictionary<string, bool> zombiesCuredDict = new Dictionary<string, bool>();

    private bool isPlayerInfected = false;

    private TMP_Text curesText;

    public GameObject[] zombies;

    public enum HealthStatus
    {
        Healthy,
        Exposed,
        EarlyInfection,
        MidInfection,
        LateInfection,
        Zombie
    }

    public HealthStatus currentHealthStatus = HealthStatus.Healthy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        curesText = GameObject.FindWithTag(Constants.curesTextTag).GetComponent<TMP_Text>();
        if (curesText == null)
            Debug.LogError("CuresText not found");

        GetZombies();
        UpdateCures(maxCures);
        UpdateHealth(HealthStatus.Healthy);
    }

    private void UpdateCures(int newAmount)
    {
        cureAmounts = newAmount;
        curesText.text = $"Cures: {newAmount.ToString()}\nStatus: {currentHealthStatus}";

        if (cureAmounts <= 0)
        {
            foreach (var zombie in zombies)
            {
                Destroy(zombie.GetComponent<ZombieAI>());
            }
            PlayerController.DisablePlayerControl();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneTransition.Instance.StartTransition(Constants.endingCharacterCreditsSceneString);
        }
    }

    private void UpdateHealth(HealthStatus status)
    {
        currentHealthStatus = status;
        string readable = Regex.Replace(status.ToString(), "(\\B[A-Z])", " $1");
        curesText.text = $"Cures: {cureAmounts.ToString()}\nStatus: {readable}";
    }

    private void GetZombies()
    {
        zombiesCuredDict.Clear();
        zombies = GameObject.FindGameObjectsWithTag(Constants.zombieTag);
        foreach (GameObject zombie in zombies)
        {
            // Initialize zombies
            /*NavMeshAgent agent = zombie.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                agent = zombie.AddComponent<NavMeshAgent>();
            }
            Rigidbody rb = zombie.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = zombie.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true;
            ZombieAI ai = zombie.GetComponent<ZombieAI>();
            if (ai == null)
            {
                ai = zombie.AddComponent<ZombieAI>();
            }*/

            // Update NavMeshAgent for unique zombie speeds (ScriptableObjects)
            // TODO:

            zombiesCuredDict.Add(zombie.name, false);
        }
    }

    public int GetAmountOfCuresLeft()
        { return cureAmounts;  }

    public void DecrementCure()
    {
        UpdateCures(cureAmounts-1);
    }

    public void InfectPlayer()
    {
        isPlayerInfected = true;

        if (currentHealthStatus < HealthStatus.Zombie)
        {
            currentHealthStatus++;
            UpdateHealth(currentHealthStatus);
        }
        else
        {
            // Turned into a zombie
        }
    }

    public string CurePlayer()
    {
        if (!isPlayerInfected)
            return "You're not currently infected.";

        if (cureAmounts <= 0)
            return "You're out of cures.";

        DecrementCure();

        currentHealthStatus = HealthStatus.Healthy;
        isPlayerInfected = false;
        UpdateHealth(currentHealthStatus);

        return "Cured used on yourself.";
    }

    public void ZombieCured(string name)
    {
        zombiesCuredDict[name] = true;
    }
}

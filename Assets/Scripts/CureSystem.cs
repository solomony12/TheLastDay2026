using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class CureSystem : MonoBehaviour
{
    public static CureSystem Instance;

    private readonly int maxCures = 10;

    private int cureAmounts;

    private Dictionary<string, bool> zombiesCuredDict = new Dictionary<string, bool>();

    private bool isPlayerInfected = false;

    private TMP_Text curesText;

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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        curesText = GameObject.FindWithTag(Constants.curesTextTag).GetComponent<TMP_Text>();
        if (curesText == null)
            Debug.LogError("CuresText not found");
    }

    private void Start()
    {
        GetZombies();
        UpdateCures(maxCures);
        UpdateHealth(HealthStatus.Healthy);
    }

    private void UpdateCures(int newAmount)
    {
        cureAmounts = newAmount;
        curesText.text = $"Cures: {newAmount.ToString()}\nStatus: {currentHealthStatus}";
    }

    private void UpdateHealth(HealthStatus status)
    {
        currentHealthStatus = status;
        string readable = Regex.Replace(status.ToString(), "(\\B[A-Z])", " $1");
        curesText.text = $"Cures: {cureAmounts.ToString()}\nStatus: {readable}";
    }

    private void GetZombies()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag(Constants.zombieTag);
        foreach (var zombie in zombies)
        {
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

    public void ZombieCured(string name)
    {
        zombiesCuredDict[name] = true;
    }
}

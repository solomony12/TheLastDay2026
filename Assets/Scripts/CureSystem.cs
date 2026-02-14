using System.Collections.Generic;
using UnityEngine;

public class CureSystem : MonoBehaviour
{
    public static CureSystem Instance;

    private int cureAmounts = 10;

    private Dictionary<string, bool> zombiesCuredDict = new Dictionary<string, bool>();

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

    private void Start()
    {
        GetZombies();
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
        { cureAmounts--; }

    public void ZombieCured(string name)
    {
        
    }
}

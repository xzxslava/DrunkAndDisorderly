using UnityEngine;
using System.Collections.Generic;

public class DrinkManager : MonoBehaviour
{
    public static DrinkManager Instance;

    [Header("All Drinks")]
    public List<DrinkData> allDrinks;

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
    }

    public DrinkData GetRandomDrink()
    {
        if (allDrinks == null || allDrinks.Count == 0)
        {
            Debug.LogWarning("No drinks available");
            return null;
        }

        return allDrinks[Random.Range(0, allDrinks.Count)];
    }

    public DrinkData GetDrinkByName(string name)
    {
        return allDrinks.Find(d => d.drinkName == name);
    }
}
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    [Header("Current")]
    public int currentGold = 500;
    public int totalEarned = 0;
    public int debt = 10000;
    public int debtPaid = 0;

    [Header("Settings")]
    public int maxGold = 99999;

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

    public void Reset(int startingGold)
    {
        currentGold = startingGold;
        totalEarned = 0;
        debt = 10000;
        debtPaid = 0;

        EventManager.OnGoldChanged?.Invoke(currentGold);
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        totalEarned += amount;

        if (currentGold > maxGold)
            currentGold = maxGold;

        EventManager.OnGoldChanged?.Invoke(currentGold);
        Debug.Log($"Gold +{amount}. Total: {currentGold}");
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount)
        {
            Debug.Log("Not enough gold");
            return false;
        }

        currentGold -= amount;
        EventManager.OnGoldChanged?.Invoke(currentGold);
        Debug.Log($"Gold -{amount}. Total: {currentGold}");
        return true;
    }

    public void PayDebt(int amount)
    {
        if (currentGold < amount)
        {
            Debug.Log("Not enough gold to pay debt!");
            // TODO: штраф за просрочку
            return;
        }

        currentGold -= amount;
        debt -= amount;
        debtPaid += amount;

        EventManager.OnGoldChanged?.Invoke(currentGold);
        EventManager.OnDebtPaid?.Invoke(debt);

        Debug.Log($"Paid {amount} debt. Remaining: {debt}");

        if (debt <= 0)
        {
            Debug.Log("DEBT PAID IN FULL!");
            // TODO: сюжетное событие
        }
    }

    public void SetGold(int amount)
    {
        currentGold = amount;
        EventManager.OnGoldChanged?.Invoke(currentGold);
    }
}
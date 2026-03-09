using UnityEngine;

public class DebtManager : MonoBehaviour
{
    public static DebtManager Instance;

    [Header("Debt Settings")]
    public int initialDebt = 10000;
    public int currentDebt;
    public int dailyPayment = 200;
    public int penaltyPerDay = 50;

    [Header("New Game+")]
    public bool isNewGamePlus = false;
    public int newGamePlusMultiplier = 2;

    private int daysWithoutPayment = 0;

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

    public void Initialize(bool newGamePlus = false)
    {
        isNewGamePlus = newGamePlus;
        currentDebt = isNewGamePlus ? initialDebt * newGamePlusMultiplier : initialDebt;
        daysWithoutPayment = 0;

        Debug.Log($"Debt initialized: {currentDebt}");
    }

    public void MakePayment(int amount)
    {
        if (amount > currentDebt)
            amount = currentDebt;

        currentDebt -= amount;
        daysWithoutPayment = 0;

        Debug.Log($"Paid {amount}. Remaining debt: {currentDebt}");
        EventManager.OnDebtPaid?.Invoke(currentDebt);

        if (currentDebt <= 0)
        {
            DebtCleared();
        }
    }

    public void MakeDailyPayment()
    {
        int payment = dailyPayment + (daysWithoutPayment * penaltyPerDay);

        if (GoldManager.Instance.currentGold >= payment)
        {
            GoldManager.Instance.SpendGold(payment);
            currentDebt -= payment;
            daysWithoutPayment = 0;

            Debug.Log($"Daily payment: {payment}. Debt left: {currentDebt}");
        }
        else
        {
            daysWithoutPayment++;
            Debug.Log($"Failed to pay daily debt. Days without payment: {daysWithoutPayment}");
            // TODO: штрафные санкции от казначея
        }

        if (currentDebt <= 0)
        {
            DebtCleared();
        }
    }

    private void DebtCleared()
    {
        Debug.Log("DEBT CLEARED! Congratulations!");
        // TODO: триггер сюжетного события
    }

    public int GetDebtPercentage()
    {
        int total = isNewGamePlus ? initialDebt * newGamePlusMultiplier : initialDebt;
        return 100 - (int)((float)currentDebt / total * 100);
    }
}
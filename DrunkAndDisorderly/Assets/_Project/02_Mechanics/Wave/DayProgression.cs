using UnityEngine;

public class DayProgression : MonoBehaviour
{
    [Header("Current Day")]
    public int currentDay = 1;

    [Header("Wave Settings")]
    public AnimationCurve customersPerWaveCurve;
    public AnimationCurve wavesPerDayCurve;
    public AnimationCurve spawnIntervalCurve;

    [Header("Debt Settings")]
    public int baseDebt = 10000;
    public int dailyDebtBase = 200;
    public int debtIncreasePerDay = 5;

    [Header("Unlock Days")]
    public int humansUnlockDay = 1;
    public int goblinsUnlockDay = 22;
    public int orcsUnlockDay = 32;
    public int elvesUnlockDay = 42;
    public int fairiesUnlockDay = 52;

    public void StartDay(int day)
    {
        currentDay = day;
        Debug.Log($"Starting day {currentDay}");
        EventManager.OnDayStarted?.Invoke(day);
    }

    public int GetCustomersPerWave()
    {
        // Базовое значение: 3 + floor(day/10)
        return 3 + Mathf.FloorToInt(currentDay / 10f);
    }

    public int GetWavesPerDay()
    {
        // Базовое значение: 3 + floor(day/20)
        return 3 + Mathf.FloorToInt(currentDay / 20f);
    }

    public float GetWaveInterval()
    {
        // Интервал уменьшается с днями: 15 - floor(day/20)
        return Mathf.Max(5f, 15f - Mathf.FloorToInt(currentDay / 20f));
    }

    public int GetDailyDebt()
    {
        return dailyDebtBase + (currentDay * debtIncreasePerDay);
    }

    public bool IsRaceUnlocked(string raceName)
    {
        switch (raceName.ToLower())
        {
            case "human":
                return currentDay >= humansUnlockDay;
            case "goblin":
                return currentDay >= goblinsUnlockDay;
            case "orc":
                return currentDay >= orcsUnlockDay;
            case "elf":
                return currentDay >= elvesUnlockDay;
            case "fairy":
                return currentDay >= fairiesUnlockDay;
            default:
                return false;
        }
    }

    public float GetPatienceMultiplier()
    {
        // Сложность растёт: терпение может уменьшаться
        return Mathf.Max(0.5f, 1f - (currentDay * 0.005f));
    }

    public float GetSpawnRateMultiplier()
    {
        // Скорость спавна растёт
        return 1f + (currentDay * 0.02f);
    }
}
using System;
using UnityEngine;

public static class EventManager
{
    // Деньги
    public static Action<int> OnGoldChanged;
    public static Action<int> OnDebtPaid;

    // Посетители
    public static Action<GameObject> OnCustomerSpawned;
    public static Action<GameObject> OnCustomerServed;
    public static Action<GameObject> OnCustomerAngry;
    public static Action<GameObject> OnCustomerLeft;

    // Эффекты
    public static Action<string, GameObject> OnEffectActivated; // effectName, target

    // Прогрессия
    public static Action<int> OnDayStarted;
    public static Action<int> OnDayEnded;
    public static Action<int> OnWaveStarted;
    public static Action OnWaveEnded;

    // Игрок
    public static Action<int> OnPlayerHit; // lives left
    public static Action OnPlayerDied;

    // Сюжет
    public static Action<string> OnDialogueTriggered; // dialogueId
}
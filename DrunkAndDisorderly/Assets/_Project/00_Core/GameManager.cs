using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Core Systems")]
    public GoldManager goldManager;
    public WaveManager waveManager;
    public DayProgression dayProgression;
    public UIManager uiManager;
    public PoolManager poolManager;

    [Header("Game State")]
    public int currentDay = 1;
    public int maxDays = 71;
    public int playerLives = 3;

    [Header("Settings")]
    public float startDelay = 2f;

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
        // Находим компоненты, если не назначены
        if (waveManager == null)
            waveManager = FindObjectOfType<WaveManager>();

        if (goldManager == null)
            goldManager = FindObjectOfType<GoldManager>();

        if (dayProgression == null)
            dayProgression = FindObjectOfType<DayProgression>();

        if (poolManager == null)
            poolManager = FindObjectOfType<PoolManager>();

        Debug.Log($"GameManager Start - waveManager: {waveManager}");

        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log($"Game started. Day {currentDay}");
        playerLives = 3;

        if (goldManager != null)
            goldManager.Reset(500);
        else
            Debug.LogError("GoldManager is null!");

        if (dayProgression != null)
            dayProgression.StartDay(currentDay);
        else
            Debug.LogError("DayProgression is null!");

        Invoke(nameof(StartFirstWave), startDelay);
    }

    private void StartFirstWave()
    {
        Debug.Log($"StartFirstWave called. waveManager null? {waveManager == null}");

        if (waveManager != null)
        {
            waveManager.StartWaves();
        }
        else
        {
            Debug.LogError("WaveManager is null! Cannot start waves.");
        }
    }

    public void TakeDamage()
    {
        playerLives--;
        Debug.Log($"Player hit! Lives left: {playerLives}");

        if (playerLives <= 0)
        {
            GameOver();
        }
        else
        {
            EventManager.OnPlayerHit?.Invoke(playerLives);
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
    }

    public void EndDay()
    {
        Debug.Log($"Day {currentDay} ended");

        if (goldManager != null && dayProgression != null)
        {
            int dailyDebt = dayProgression.GetDailyDebt();
            goldManager.PayDebt(dailyDebt);
        }

        currentDay++;

        if (currentDay > maxDays)
        {
            HandleGameComplete();
        }
        else
        {
            uiManager?.ShowUpgradeShop();
            dayProgression?.StartDay(currentDay);
        }
    }

    private void HandleGameComplete()
    {
        Debug.Log("Main campaign completed!");
    }
}
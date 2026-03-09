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
        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log($"Game started. Day {currentDay}");
        playerLives = 3;
        goldManager?.Reset(500);
        dayProgression?.StartDay(currentDay);
        Invoke(nameof(StartFirstWave), startDelay);
    }

    private void StartFirstWave()
    {
        waveManager?.StartWaves();
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
            // Визуальный/звуковой эффект
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        // Заглушка: рестарт или возврат в меню
    }

    public void EndDay()
    {
        Debug.Log($"Day {currentDay} ended");

        // Платёж казначею
        int dailyDebt = dayProgression.GetDailyDebt();
        goldManager.PayDebt(dailyDebt);

        currentDay++;

        if (currentDay > maxDays)
        {
            HandleGameComplete();
        }
        else
        {
            uiManager?.ShowUpgradeShop();
            dayProgression.StartDay(currentDay);
        }
    }

    private void HandleGameComplete()
    {
        Debug.Log("Main campaign completed! New Game+ unlocked?");
        // TODO: разблокировка New Game+
    }
}
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Top Panel")]
    public Text goldText;
    public Text debtText;
    public Text dayText;

    [Header("Shop")]
    public GameObject shopPanel;
    public Button openShopButton;
    public Button closeShopButton;
    public Transform shopContentArea; // Ссылка на Content из Scroll View
    public GameObject shopItemPrefab; // Префаб товара

    [Header("Wave")]
    public Button nextWaveButton;
    public Text waveText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public Button restartButton;

    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public Text dialogueSpeakerText;
    public Text dialogueContentText;
    public Image dialoguePortrait;

    [Header("Settings")]
    public Color positiveGoldColor = Color.yellow;
    public Color negativeGoldColor = Color.red;

    private WaveManager waveManager;

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

        waveManager = FindObjectOfType<WaveManager>();
    }

    private void Start()
    {
        SubscribeEvents();
        UpdateAllUI();

        if (shopPanel != null)
            shopPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (openShopButton != null)
            openShopButton.onClick.AddListener(OpenShop);

        if (closeShopButton != null)
            closeShopButton.onClick.AddListener(CloseShop);

        if (nextWaveButton != null)
            nextWaveButton.onClick.AddListener(OnNextWaveClicked);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        // Заполняем магазин товарами
        PopulateShop();
    }

    private void SubscribeEvents()
    {
        EventManager.OnGoldChanged += UpdateGold;
        EventManager.OnDebtPaid += UpdateDebt;
        EventManager.OnDayStarted += UpdateDay;
        EventManager.OnWaveStarted += UpdateWave;
        EventManager.OnPlayerDied += ShowGameOver;
    }

    private void OnDestroy()
    {
        EventManager.OnGoldChanged -= UpdateGold;
        EventManager.OnDebtPaid -= UpdateDebt;
        EventManager.OnDayStarted -= UpdateDay;
        EventManager.OnWaveStarted -= UpdateWave;
        EventManager.OnPlayerDied -= ShowGameOver;
    }

    private void UpdateAllUI()
    {
        if (GoldManager.Instance != null)
        {
            UpdateGold(GoldManager.Instance.currentGold);
            UpdateDebt(GoldManager.Instance.debt);
        }

        if (GameManager.Instance != null)
        {
            UpdateDay(GameManager.Instance.currentDay);
        }
    }

    private void UpdateGold(int gold)
    {
        if (goldText != null)
        {
            goldText.text = gold.ToString();
            goldText.color = gold >= 0 ? positiveGoldColor : negativeGoldColor;
        }
    }

    private void UpdateDebt(int debt)
    {
        if (debtText != null)
        {
            debtText.text = debt.ToString();
        }
    }

    private void UpdateDay(int day)
    {
        if (dayText != null)
        {
            dayText.text = $"Day {day}";
        }
    }

    private void UpdateWave(int wave)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {wave + 1}";
        }
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            Time.timeScale = 0f; // Пауза
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Time.timeScale = 1f; // Возобновляем
        }
    }

    private void OnNextWaveClicked()
    {
        if (waveManager != null)
        {
            // Если есть метод ForceNextWave, вызываем его
            // waveManager.ForceNextWave();
        }

        if (nextWaveButton != null)
            nextWaveButton.gameObject.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    // Методы для диалогов (исправляют ошибки)
    public void ShowDialogue(string speaker, string text, Sprite portrait = null)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);

            if (dialogueSpeakerText != null)
                dialogueSpeakerText.text = speaker;

            if (dialogueContentText != null)
                dialogueContentText.text = text;

            if (dialoguePortrait != null && portrait != null)
                dialoguePortrait.sprite = portrait;
        }
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    // Метод для ShowUpgradeShop (исправляет ошибку в GameManager)
    public void ShowUpgradeShop()
    {
        OpenShop();
    }

    // Заполнение магазина товарами
    private void PopulateShop()
    {
        if (shopContentArea == null || shopItemPrefab == null) return;

        // Очищаем существующие товары
        foreach (Transform child in shopContentArea)
        {
            Destroy(child.gameObject);
        }

        // Создаём товары
        CreateShopItem("Ускорение крана", 500, UpgradeType.TapSpeed, 1);
        CreateShopItem("Терпение +", 300, UpgradeType.CustomerPatience, 2);
        CreateShopItem("Цены +20%", 400, UpgradeType.DrinkPrice, 20);
        CreateShopItem("Новый напиток", 1000, UpgradeType.NewDrink, 0);
    }

    private void CreateShopItem(string name, int price, UpgradeType type, int value)
    {
        GameObject itemObj = Instantiate(shopItemPrefab, shopContentArea);
        ShopItem item = itemObj.GetComponent<ShopItem>();

        if (item != null)
        {
            item.Initialize(name, price, type, value);
        }
    }
}
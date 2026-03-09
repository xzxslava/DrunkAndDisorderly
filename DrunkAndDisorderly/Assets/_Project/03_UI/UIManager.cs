using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Gold UI")]
    public Text goldText;
    public Text debtText;

    [Header("Day UI")]
    public Text dayText;
    public Text waveText;
    public Button nextWaveButton;

    [Header("Resource UI")]
    public Transform resourcePanel;
    public GameObject resourceIconPrefab;

    [Header("Upgrade Shop")]
    public GameObject upgradeShopPanel;
    public Transform upgradeContainer;
    public GameObject upgradeItemPrefab;

    [Header("Dialogue")]
    public GameObject dialoguePanel;
    public Text dialogueSpeakerText;
    public Text dialogueContentText;
    public Image dialoguePortrait;

    [Header("Player Status")]
    public Image[] lifeIcons; // 3 иконки жизней
    public Slider dayProgressSlider;

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

    private void Start()
    {
        SubscribeEvents();
        UpdateAllUI();
    }

    private void SubscribeEvents()
    {
        EventManager.OnGoldChanged += UpdateGold;
        EventManager.OnDebtPaid += UpdateDebt;
        EventManager.OnDayStarted += UpdateDay;
        EventManager.OnWaveStarted += UpdateWave;
        EventManager.OnPlayerHit += UpdateLives;
    }

    private void OnDestroy()
    {
        EventManager.OnGoldChanged -= UpdateGold;
        EventManager.OnDebtPaid -= UpdateDebt;
        EventManager.OnDayStarted -= UpdateDay;
        EventManager.OnWaveStarted -= UpdateWave;
        EventManager.OnPlayerHit -= UpdateLives;
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
            UpdateLives(GameManager.Instance.playerLives);
        }
    }

    private void UpdateGold(int gold)
    {
        if (goldText != null)
            goldText.text = gold.ToString();
    }

    private void UpdateDebt(int debt)
    {
        if (debtText != null)
            debtText.text = debt.ToString();
    }

    private void UpdateDay(int day)
    {
        if (dayText != null)
            dayText.text = $"Day {day}";

        if (dayProgressSlider != null)
        {
            float progress = (float)day / 71f;
            dayProgressSlider.value = progress;
        }
    }

    private void UpdateWave(int wave)
    {
        if (waveText != null)
            waveText.text = $"Wave {wave + 1}";
    }

    private void UpdateLives(int lives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
                lifeIcons[i].enabled = i < lives;
        }
    }

    public void ShowUpgradeShop()
    {
        if (upgradeShopPanel != null)
            upgradeShopPanel.SetActive(true);
    }

    public void HideUpgradeShop()
    {
        if (upgradeShopPanel != null)
            upgradeShopPanel.SetActive(false);
    }

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

    public void OnNextWaveButtonClick()
    {
        nextWaveButton.gameObject.SetActive(false);
        // TODO: сообщить WaveManager о ручном запуске волны
    }
}
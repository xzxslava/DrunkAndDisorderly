using UnityEngine;
using UnityEngine.UI;

public class CustomerPatience : MonoBehaviour
{
    [Header("Settings")]
    public float maxPatience = 10f;
    public float currentPatience;
    public float patienceRate = 1f;

    [Header("UI")]
    public GameObject patienceBarPrefab; // Теперь это Canvas префаб
    public float barHeight = 2.5f;

    [Header("Colors")]
    public Color fullColor = Color.green;
    public Color midColor = Color.yellow;
    public Color emptyColor = Color.red;

    private GameObject activeBarCanvas;
    private Slider barSlider;
    private Image fillImage;
    private CustomerBase customer;
    private bool isWaiting = false;
    private bool isInitialized = false;

    private void Awake()
    {
        customer = GetComponent<CustomerBase>();
    }

    private void Start()
    {
        Invoke(nameof(DelayedInitialize), 1.5f);
    }

    private void DelayedInitialize()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (customer != null && customer.race != null)
        {
            maxPatience = customer.race.basePatience;
            patienceRate = customer.race.patienceRate;
            currentPatience = maxPatience;
            isInitialized = true;

            Debug.Log($"Patience initialized: {maxPatience}s, rate: {patienceRate}");
            CreatePatienceBar();
            isWaiting = true;
        }
        else
        {
            Debug.LogWarning("Customer or race is null, retrying...");
            Invoke(nameof(DelayedInitialize), 0.5f);
        }
    }

    private void Update()
    {
        if (!isInitialized || !isWaiting || customer == null || customer.isLeaving) return;

        currentPatience -= Time.deltaTime * patienceRate;

        if (barSlider != null)
        {
            float percentage = currentPatience / maxPatience;
            barSlider.value = Mathf.Clamp01(percentage);

            if (fillImage != null)
            {
                if (percentage > 0.6f)
                    fillImage.color = fullColor;
                else if (percentage > 0.3f)
                    fillImage.color = midColor;
                else
                    fillImage.color = emptyColor;
            }
        }

        if (currentPatience <= 0 && !customer.isAngry)
        {
            Debug.Log($"{customer.race?.raceName} разозлился!");
            customer.GetAngry();
        }
    }

    public void ReducePatience(float amount)
    {
        currentPatience -= amount;
        Debug.Log($"Patience reduced by {amount}, current: {currentPatience}");

        if (currentPatience <= 0 && !customer.isAngry)
        {
            customer.GetAngry();
        }
    }

    private void CreatePatienceBar()
    {
        if (patienceBarPrefab == null)
        {
            Debug.LogError("Patience bar prefab is null!");
            return;
        }

        // Создаём Canvas как дочерний объект посетителя
        activeBarCanvas = Instantiate(patienceBarPrefab, transform);

        // Устанавливаем позицию над головой
        activeBarCanvas.transform.localPosition = new Vector3(0, barHeight, 0);

        // Находим Slider в дочерних объектах
        barSlider = activeBarCanvas.GetComponentInChildren<Slider>();
        if (barSlider != null)
        {
            barSlider.maxValue = 1f;
            barSlider.value = 1f;

            // Находим изображение заливки
            fillImage = barSlider.fillRect?.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("No Slider found in patience bar prefab!");
        }
    }

    public void ResetPatience()
    {
        currentPatience = maxPatience;
        if (barSlider != null)
            barSlider.value = 1f;
    }

    private void OnDestroy()
    {
        if (activeBarCanvas != null)
            Destroy(activeBarCanvas);
    }
}
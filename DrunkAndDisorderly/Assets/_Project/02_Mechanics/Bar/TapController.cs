using UnityEngine;
using UnityEngine.UI;

public class TapController : MonoBehaviour
{
    [Header("Drink Settings")]
    public DrinkData drinkType;
    public GameObject drinkPrefab;
    public Transform pourPosition;

    [Header("Progress UI")]
    public GameObject progressBarPrefab;
    public float progressBarHeight = 1.5f;

    [Header("State")]
    public bool isBusy = false;
    public bool hasDrinkReady = false;
    public GameObject currentDrink;

    private GameObject progressBar;
    private Slider progressSlider;
    private float currentProgress;
    private float totalProgress;
    private Material originalMaterial;
    private Renderer tapRenderer;

    private void Start()
    {
        tapRenderer = GetComponent<Renderer>();
        if (tapRenderer != null)
            originalMaterial = tapRenderer.material;

        if (pourPosition == null)
            pourPosition = transform;
    }

    private void OnMouseDown()
    {
        Debug.Log($"Tap clicked: {drinkType?.drinkName}");
        TryStartPouring();
    }

    public void TryStartPouring()
    {
        if (isBusy || hasDrinkReady) return;

        // Проверяем, есть ли ресурсы
        if (drinkType != null)
        {
            // TODO: проверить ResourceManager
            StartPouring();
        }
    }

    private void StartPouring()
    {
        isBusy = true;
        currentProgress = 0f;
        totalProgress = drinkType != null ? drinkType.preparationTime : 2f;

        CreateProgressBar();

        // Подсветка красным (занят)
        if (tapRenderer != null)
            tapRenderer.material.color = Color.red;

        Debug.Log($"Started pouring {drinkType?.drinkName}");
    }

    private void Update()
    {
        if (!isBusy) return;

        currentProgress += Time.deltaTime;

        // Обновляем прогресс-бар
        if (progressSlider != null)
        {
            progressSlider.value = currentProgress / totalProgress;
        }

        // Проверяем завершение
        if (currentProgress >= totalProgress)
        {
            FinishPouring();
        }
    }

    private void FinishPouring()
    {
        isBusy = false;
        hasDrinkReady = true;

        DestroyProgressBar();

        // Создаём напиток
        if (drinkPrefab != null && pourPosition != null)
        {
            currentDrink = Instantiate(drinkPrefab, pourPosition.position, Quaternion.identity);

            // Настраиваем напиток
            DrinkProduct product = currentDrink.GetComponent<DrinkProduct>();
            if (product == null)
                product = currentDrink.AddComponent<DrinkProduct>();

            product.Initialize(drinkType, this);
        }

        // Подсветка зелёным (готово)
        if (tapRenderer != null)
            tapRenderer.material.color = Color.green;

        Debug.Log($"Finished pouring {drinkType?.drinkName}");
    }

    private void CreateProgressBar()
    {
        if (progressBarPrefab == null) return;

        progressBar = Instantiate(progressBarPrefab, transform.position + Vector3.up * progressBarHeight, Quaternion.identity, transform);
        progressSlider = progressBar.GetComponentInChildren<Slider>();

        if (progressSlider != null)
        {
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
        }

        // Добавляем Billboard
        if (progressBar.GetComponent<Billboard>() == null)
            progressBar.AddComponent<Billboard>();
    }

    private void DestroyProgressBar()
    {
        if (progressBar != null)
            Destroy(progressBar);
    }

    public void DrinkTaken()
    {
        hasDrinkReady = false;
        currentDrink = null;

        // Возвращаем обычный цвет
        if (tapRenderer != null)
            tapRenderer.material.color = originalMaterial != null ? originalMaterial.color : Color.white;
    }
}
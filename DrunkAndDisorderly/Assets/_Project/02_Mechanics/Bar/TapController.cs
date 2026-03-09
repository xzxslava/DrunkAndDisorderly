using UnityEngine;

public class TapController : EquipmentBase
{
    [Header("Drink Settings")]
    public DrinkData drinkType;
    public GameObject drinkPrefab;
    public Transform pourPosition;

    [Header("Progress UI")]
    public GameObject progressBarPrefab;
    private GameObject activeProgressBar;
    private UnityEngine.UI.Slider progressSlider;

    protected override void Start()
    {
        base.Start();
        equipmentType = EquipmentType.Tap;

        if (drinkType != null)
        {
            equipmentName = drinkType.drinkName + " Tap";
            currentActionTime = drinkType.preparationTime;
            requiredResource = drinkType.drinkName; // предполагаем, что ресурс называется так же
        }
    }

    public override bool TryInteract()
    {
        // Проверяем, есть ли свободный слот для продукта
        if (!isInteractable) return false;
        if (currentState != EquipmentState.Idle) return false;

        return base.TryInteract();
    }

    protected override void StartAction()
    {
        base.StartAction();
        ShowProgressBar();

        // Звук наливания
        if (drinkType != null && drinkType.pourSound != null)
        {
            AudioSource.PlayClipAtPoint(drinkType.pourSound, transform.position);
        }
    }

    protected override System.Collections.IEnumerator ActionCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < currentActionTime)
        {
            elapsed += Time.deltaTime;

            if (progressSlider != null)
            {
                progressSlider.value = elapsed / currentActionTime;
            }

            yield return null;
        }

        CompleteAction();
    }

    protected override void CreateProduct()
    {
        if (drinkPrefab != null && pourPosition != null)
        {
            GameObject drink = Instantiate(drinkPrefab, pourPosition.position, Quaternion.identity);

            // Настраиваем продукт
            DrinkProduct product = drink.GetComponent<DrinkProduct>();
            if (product == null)
                product = drink.AddComponent<DrinkProduct>();

            product.Initialize(drinkType);

            Debug.Log($"Poured {drinkType.drinkName}");
        }

        HideProgressBar();
    }

    private void ShowProgressBar()
    {
        if (progressBarPrefab == null) return;

        activeProgressBar = Instantiate(progressBarPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity, transform);
        progressSlider = activeProgressBar.GetComponent<UnityEngine.UI.Slider>();

        if (progressSlider != null)
        {
            progressSlider.maxValue = 1f;
            progressSlider.value = 0f;
        }

        // Добавляем Billboard
        if (activeProgressBar.GetComponent<Billboard>() == null)
        {
            activeProgressBar.AddComponent<Billboard>();
        }
    }

    private void HideProgressBar()
    {
        if (activeProgressBar != null)
        {
            Destroy(activeProgressBar);
        }
    }
}
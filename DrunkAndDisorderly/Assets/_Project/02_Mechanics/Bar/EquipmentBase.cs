using UnityEngine;

public enum EquipmentType
{
    Tap,           // кран для напитков
    KitchenTable,  // стол для закусок
    CashRegister,  // касса
    Barrel,        // бочка для хранения
    Upgradeable    // улучшаемый объект
}

public enum EquipmentState
{
    Idle,      // готов к работе
    Busy,      // выполняет действие
    Empty,     // закончились ресурсы
    Broken     // сломано (после драки)
}

public class EquipmentBase : MonoBehaviour
{
    [Header("Basic Info")]
    public string equipmentName;
    public EquipmentType equipmentType;
    public int level = 1;

    [Header("Settings")]
    public float baseActionTime = 2f;
    public float currentActionTime;
    public int maxLevel = 3;

    [Header("Resources")]
    public string requiredResource; // например, "Mead", "Grain"
    public int resourceCost = 1;

    [Header("State")]
    public EquipmentState currentState = EquipmentState.Idle;
    public bool isInteractable = true;

    [Header("Visuals")]
    public GameObject[] levelModels; // модели для каждого уровня
    public Material idleMaterial;
    public Material busyMaterial;
    public Material emptyMaterial;

    protected Renderer objectRenderer;
    protected float actionTimer;

    protected virtual void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        currentActionTime = baseActionTime;
        UpdateVisuals();
    }

    public virtual bool TryInteract()
    {
        if (!isInteractable) return false;
        if (currentState != EquipmentState.Idle) return false;

        // Проверяем наличие ресурсов
        if (!string.IsNullOrEmpty(requiredResource))
        {
            if (ResourceManager.Instance.GetResourceAmount(requiredResource) < resourceCost)
            {
                currentState = EquipmentState.Empty;
                UpdateVisuals();
                return false;
            }

            // Потребляем ресурс
            ResourceManager.Instance.ConsumeResource(requiredResource, resourceCost);
        }

        StartAction();
        return true;
    }

    protected virtual void StartAction()
    {
        currentState = EquipmentState.Busy;
        actionTimer = currentActionTime;
        UpdateVisuals();

        // Запускаем прогресс
        StartCoroutine(ActionCoroutine());
    }

    protected virtual System.Collections.IEnumerator ActionCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < currentActionTime)
        {
            elapsed += Time.deltaTime;
            // Здесь можно обновлять прогресс-бар
            yield return null;
        }

        CompleteAction();
    }

    protected virtual void CompleteAction()
    {
        currentState = EquipmentState.Idle;
        UpdateVisuals();

        // Создаём готовый продукт
        CreateProduct();
    }

    protected virtual void CreateProduct()
    {
        // Создаём объект напитка/закуски для взаимодействия
        Debug.Log($"{equipmentName} created product");
        // TODO: спавн интерактивного объекта
    }

    public virtual bool Upgrade()
    {
        if (level >= maxLevel) return false;

        level++;
        currentActionTime = baseActionTime / level; // ускорение с уровнем

        if (levelModels.Length >= level && levelModels[level - 1] != null)
        {
            // Скрываем старую модель, показываем новую
            for (int i = 0; i < levelModels.Length; i++)
            {
                if (levelModels[i] != null)
                    levelModels[i].SetActive(i == level - 1);
            }
        }

        Debug.Log($"{equipmentName} upgraded to level {level}");
        return true;
    }

    protected virtual void UpdateVisuals()
    {
        if (objectRenderer == null) return;

        switch (currentState)
        {
            case EquipmentState.Idle:
                objectRenderer.material = idleMaterial;
                break;
            case EquipmentState.Busy:
                objectRenderer.material = busyMaterial;
                break;
            case EquipmentState.Empty:
                objectRenderer.material = emptyMaterial;
                break;
        }
    }

    public void SetBroken(bool broken)
    {
        if (broken)
        {
            currentState = EquipmentState.Broken;
            isInteractable = false;
        }
        else
        {
            currentState = EquipmentState.Idle;
            isInteractable = true;
        }
        UpdateVisuals();
    }
}
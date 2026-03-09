using UnityEngine;
using System.Collections;

public class DrinkProduct : MonoBehaviour
{
    public DrinkData drinkData;
    public TapController sourceTap;

    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;

    private bool isPickedUp = false;
    private Renderer objectRenderer;
    private Coroutine pulseCoroutine;
    private Color originalColor;
    private bool isHighlighted = false;
    private Material uniqueMaterial;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer found on DrinkProduct! Please add a MeshRenderer or SpriteRenderer.");
            return;
        }

        // Добавляем коллайдер, если его нет
        if (GetComponent<Collider>() == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.5f, 0.5f, 0.5f);
            Debug.Log("Added BoxCollider");
        }
    }

    public void Initialize(DrinkData data, TapController tap)
    {
        if (data == null)
        {
            Debug.LogError("DrinkData is null in Initialize!");
            return;
        }

        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<Renderer>();
            if (objectRenderer == null)
            {
                Debug.LogError("Cannot initialize: No Renderer found!");
                return;
            }
        }

        drinkData = data;
        sourceTap = tap;

        // Создаём уникальный материал для этого объекта
        uniqueMaterial = new Material(objectRenderer.sharedMaterial);
        uniqueMaterial.color = data.drinkColor;
        objectRenderer.material = uniqueMaterial;
        originalColor = data.drinkColor;

        Debug.Log($"Created unique material with color: {originalColor} for {data.drinkName}");

        // Запускаем пульсацию
        pulseCoroutine = StartCoroutine(PulseEffect());

        Debug.Log($"Drink initialized: {data?.drinkName}");
    }

    private IEnumerator PulseEffect()
    {
        if (uniqueMaterial == null)
        {
            Debug.LogError("PulseEffect: uniqueMaterial is null!");
            yield break;
        }

        while (!isPickedUp && !isHighlighted)
        {
            float t = Mathf.PingPong(Time.time * 2f, 1f);
            Color pulseColor = originalColor * (1f + t * 0.3f);
            uniqueMaterial.color = pulseColor;
            yield return null;
        }
    }

    private void OnMouseEnter()
    {
        if (isPickedUp) return;

        Debug.Log($"Mouse entered {drinkData?.drinkName}");

        if (uniqueMaterial == null)
        {
            Debug.LogError($"uniqueMaterial is null in OnMouseEnter for {drinkData?.drinkName}! Material was not created properly.");
            return;
        }

        isHighlighted = true;

        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        uniqueMaterial.color = highlightColor;
        Debug.Log($"Highlight color set to {highlightColor}");
    }

    private void OnMouseExit()
    {
        if (isPickedUp) return;

        Debug.Log($"Mouse exited {drinkData?.drinkName}");

        if (uniqueMaterial == null)
        {
            Debug.LogError($"uniqueMaterial is null in OnMouseExit for {drinkData?.drinkName}!");
            return;
        }

        isHighlighted = false;
        uniqueMaterial.color = originalColor;
        Debug.Log($"Color restored to {originalColor}");

        // Возобновляем пульсацию
        pulseCoroutine = StartCoroutine(PulseEffect());
    }

    private void OnMouseDown()
    {
        Debug.Log($"Mouse down on {drinkData?.drinkName}");
        TryPickUp();
    }

    public void TryPickUp()
    {
        if (isPickedUp) return;

        isPickedUp = true;
        Debug.Log($"Picked up {drinkData?.drinkName}");

        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        if (uniqueMaterial != null)
        {
            uniqueMaterial.color = highlightColor;
            Debug.Log("Pickup highlight color set");
        }

        // Сообщаем InputManager, что выбран этот напиток
        if (InputManager.Instance != null)
        {
            InputManager.Instance.SelectDrink(gameObject);
        }

        // Запускаем таймер на уничтожение, если не отдадим посетителю
        StartCoroutine(DestroyAfterDelay(5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isPickedUp) yield break; // уже не актуально

        Debug.Log($"Drink {drinkData?.drinkName} expired");

        if (sourceTap != null)
            sourceTap.DrinkTaken();

        Destroy(gameObject);
    }

    public void DeliverToCustomer(CustomerBase customer)
    {
        if (customer == null || drinkData == null || isPickedUp == false)
        {
            Debug.LogWarning($"Cannot deliver: customer null={customer == null}, drinkData null={drinkData == null}, isPickedUp={isPickedUp}");
            return;
        }

        Debug.Log($"Delivering {drinkData.drinkName} to {customer.race?.raceName}");

        if (pulseCoroutine != null)
            StopCoroutine(pulseCoroutine);

        customer.ServeDrink(drinkData);

        if (sourceTap != null)
            sourceTap.DrinkTaken();

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (uniqueMaterial != null)
        {
            Destroy(uniqueMaterial);
        }

        if (sourceTap != null && sourceTap.currentDrink == gameObject)
        {
            sourceTap.DrinkTaken();
        }
    }
}
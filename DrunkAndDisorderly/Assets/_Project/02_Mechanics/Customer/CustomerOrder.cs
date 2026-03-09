using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    [Header("UI")]
    public GameObject orderIconPrefab;
    public Transform iconPosition;

    private DrinkData currentDrink;
    private GameObject activeIcon;
    private CustomerBase customer;

    private void Awake()
    {
        customer = GetComponent<CustomerBase>();
        if (iconPosition == null)
            iconPosition = transform; // запасной вариант
    }

    public void GenerateOrder()
    {
        if (customer.race == null)
        {
            Debug.LogWarning("Race not set, cannot generate order");
            return;
        }

        // Выбираем случайный напиток из предпочтений или обычных
        if (customer.race.preferredDrinks.Length > 0 && Random.value > 0.5f)
        {
            currentDrink = customer.race.preferredDrinks[Random.Range(0, customer.race.preferredDrinks.Length)];
        }
        else if (customer.race.regularDrinks.Length > 0)
        {
            currentDrink = customer.race.regularDrinks[Random.Range(0, customer.race.regularDrinks.Length)];
        }

        customer.currentOrder = currentDrink;
        ShowOrderIcon();
    }

    private void ShowOrderIcon()
    {
        if (currentDrink == null || orderIconPrefab == null) return;

        activeIcon = Instantiate(orderIconPrefab, iconPosition.position, Quaternion.identity, transform);

        // Настраиваем иконку (спрайт, цвет и т.д.)
        var iconRenderer = activeIcon.GetComponent<SpriteRenderer>();
        if (iconRenderer != null && currentDrink.icon != null)
        {
            iconRenderer.sprite = currentDrink.icon;
        }

        // Добавляем Billboard, если нужно
        if (activeIcon.GetComponent<Billboard>() == null)
        {
            activeIcon.AddComponent<Billboard>();
        }
    }

    public void HideOrderIcon()
    {
        if (activeIcon != null)
        {
            Destroy(activeIcon);
        }
    }

    public DrinkData GetCurrentOrder()
    {
        return currentDrink;
    }
}
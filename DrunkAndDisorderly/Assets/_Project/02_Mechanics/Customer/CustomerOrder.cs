using UnityEngine;
using System.Collections.Generic;

public class CustomerOrder : MonoBehaviour
{
    [Header("Settings")]
    public GameObject orderIconPrefab;
    public Transform iconPosition;
    public float iconHeight = 1.8f;

    [Header("Current Order")]
    public DrinkData currentDrink;

    private GameObject activeIcon;
    private CustomerBase customer;
    private SpriteRenderer iconRenderer;

    private void Awake()
    {
        customer = GetComponent<CustomerBase>();

        if (iconPosition == null)
        {
            GameObject iconPosObj = new GameObject("IconPosition");
            iconPosObj.transform.SetParent(transform);
            iconPosObj.transform.localPosition = new Vector3(0, iconHeight, 0);
            iconPosition = iconPosObj.transform;
        }
    }

    private void Start()
    {
        // Ждём, пока посетитель сядет и инициализируется раса
        Invoke(nameof(GenerateOrder), 1.5f);
    }

    public void GenerateOrder()
    {
        if (customer == null || customer.race == null)
        {
            Debug.LogWarning("Customer or race is null, cannot generate order");
            return;
        }

        // ВЫБИРАЕМ НАПИТОК ИЗ ПРЕДПОЧТЕНИЙ РАСЫ
        if (customer.race.preferredDrinks != null && customer.race.preferredDrinks.Length > 0)
        {
            // Выбираем случайный напиток из предпочтений
            int randomIndex = Random.Range(0, customer.race.preferredDrinks.Length);
            currentDrink = customer.race.preferredDrinks[randomIndex];
        }
        else
        {
            // Если нет предпочтений, берём случайный из всех (как запасной вариант)
            if (DrinkManager.Instance != null && DrinkManager.Instance.allDrinks.Count > 0)
            {
                List<DrinkData> availableDrinks = new List<DrinkData>();
                int currentDay = GameManager.Instance != null ? GameManager.Instance.currentDay : 1;

                foreach (var drink in DrinkManager.Instance.allDrinks)
                {
                    if (drink.unlockDay <= currentDay)
                        availableDrinks.Add(drink);
                }

                if (availableDrinks.Count > 0)
                    currentDrink = availableDrinks[Random.Range(0, availableDrinks.Count)];
            }
        }

        if (currentDrink == null)
        {
            Debug.LogError("No drink available for order!");
            return;
        }

        customer.currentOrder = currentDrink;
        ShowOrderIcon();

        Debug.Log($"{customer.race.raceName} заказал {currentDrink.drinkName}");
    }

    private void ShowOrderIcon()
    {
        if (orderIconPrefab == null || currentDrink == null) return;

        activeIcon = Instantiate(orderIconPrefab, iconPosition.position, Quaternion.identity, iconPosition);

        iconRenderer = activeIcon.GetComponent<SpriteRenderer>();
        if (iconRenderer != null && currentDrink.icon != null)
        {
            iconRenderer.sprite = currentDrink.icon;
        }

        // Настраиваем масштаб (если нужно)
        activeIcon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Добавляем Billboard
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

    public bool IsCorrectDrink(DrinkData drink)
    {
        return drink == currentDrink;
    }
}
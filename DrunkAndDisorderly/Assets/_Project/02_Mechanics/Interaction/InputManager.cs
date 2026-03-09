using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Header("Current Selection")]
    public GameObject selectedDrink;
    public CustomerBase selectedCustomer;

    [Header("Settings")]
    public LayerMask drinkLayer;
    public LayerMask customerLayer;
    public float maxRayDistance = 100f;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Для ПК (мышь)
        if (Input.GetMouseButtonDown(0))
        {
            HandleTap(Input.mousePosition);
        }

        // Для мобилок (тач)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                HandleTap(touch.position);
            }
        }
    }

    private void HandleTap(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Проверяем, это напиток?
            DrinkProduct drink = hitObject.GetComponent<DrinkProduct>();
            if (drink != null)
            {
                // Проверяем слой, если он задан
                if (drinkLayer.value == 0 || ((1 << hitObject.layer) & drinkLayer.value) != 0)
                {
                    SelectDrink(drink.gameObject);
                }
                return;
            }

            // Проверяем, это посетитель?
            CustomerBase customer = hitObject.GetComponent<CustomerBase>();
            if (customer != null)
            {
                // Проверяем слой, если он задан
                if (customerLayer.value == 0 || ((1 << hitObject.layer) & customerLayer.value) != 0)
                {
                    if (selectedDrink != null)
                    {
                        // Пытаемся отдать напиток
                        DeliverDrinkToCustomer(customer);
                    }
                    else
                    {
                        // Просто выделяем посетителя (для информации)
                        SelectCustomer(customer);
                    }
                }
                return;
            }

            // Кликнули в пустоту — сбрасываем выделение
            if (selectedDrink != null)
            {
                DeselectDrink();
            }
        }
        else
        {
            // Кликнули в пустоту
            if (selectedDrink != null)
            {
                DeselectDrink();
            }
        }
    }

    public void SelectDrink(GameObject drink) // ИЗМЕНЕНО: private -> public
    {
        // Снимаем выделение с предыдущего напитка
        if (selectedDrink != null)
        {
            DeselectDrink();
        }

        selectedDrink = drink;
        Debug.Log($"Selected drink: {drink.name}");

        // Визуально выделяем напиток (подсветка уже есть в DrinkProduct)
    }

    private void SelectCustomer(CustomerBase customer)
    {
        selectedCustomer = customer;
        Debug.Log($"Selected customer: {customer.race?.raceName}");

        // TODO: показать информацию о заказе
    }

    private void DeliverDrinkToCustomer(CustomerBase customer)
    {
        if (selectedDrink == null) return;

        DrinkProduct drinkProduct = selectedDrink.GetComponent<DrinkProduct>();
        if (drinkProduct == null) return;

        Debug.Log($"Attempting to deliver {drinkProduct.drinkData?.drinkName} to {customer.race?.raceName}");

        // Отдаём напиток
        drinkProduct.DeliverToCustomer(customer);

        // Сбрасываем выделение
        selectedDrink = null;
    }

    private void DeselectDrink()
    {
        if (selectedDrink == null) return;

        Debug.Log($"Deselected drink: {selectedDrink.name}");
        selectedDrink = null;
    }
}
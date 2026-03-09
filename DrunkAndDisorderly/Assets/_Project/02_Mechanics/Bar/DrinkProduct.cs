using UnityEngine;

public class DrinkProduct : MonoBehaviour
{
    [Header("Drink Data")]
    public DrinkData drinkData;

    [Header("State")]
    public bool isReady = true;
    public float lifeTime = 30f; // исчезнет через 30 сек, если не забрали

    private float timer;

    private void Start()
    {
        timer = lifeTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(DrinkData data)
    {
        drinkData = data;

        // Настраиваем внешний вид
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = data.drinkColor;
        }

        // Подсвечиваем
        StartCoroutine(PulseEffect());
    }

    private System.Collections.IEnumerator PulseEffect()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) yield break;

        Color originalColor = rend.material.color;
        Color pulseColor = originalColor * 1.5f;

        while (isReady)
        {
            // Пульсация
            float t = Mathf.PingPong(Time.time * 2f, 1f);
            rend.material.color = Color.Lerp(originalColor, pulseColor, t);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        // Для ПК-тестирования
        TryPickUp();
    }

    public void TryPickUp()
    {
        // Здесь будет логика: игрок берёт напиток
        Debug.Log($"Picked up {drinkData?.drinkName}");

        // Подсвечиваем, что продукт выбран
        GetComponent<Renderer>().material.color = Color.yellow;

        // TODO: начать перетаскивание
    }

    public void DeliverToCustomer(CustomerBase customer)
    {
        if (customer != null && drinkData != null)
        {
            customer.ServeDrink(drinkData);
            Destroy(gameObject);
        }
    }
}
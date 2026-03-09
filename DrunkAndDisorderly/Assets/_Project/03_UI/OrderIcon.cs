using UnityEngine;
using UnityEngine.UI;

public class OrderIcon : MonoBehaviour
{
    [Header("References")]
    public Image iconImage;
    public Image backgroundImage;

    [Header("Settings")]
    public float floatSpeed = 1f;
    public float floatHeight = 0.2f;

    private Vector3 startPosition;
    private DrinkData currentDrink;

    private void Start()
    {
        startPosition = transform.localPosition;

        // Добавляем Billboard, если нет
        if (GetComponent<Billboard>() == null)
        {
            gameObject.AddComponent<Billboard>();
        }
    }

    private void Update()
    {
        // Эффект парения
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }

    public void SetDrink(DrinkData drink)
    {
        currentDrink = drink;

        if (iconImage != null && drink != null && drink.icon != null)
        {
            iconImage.sprite = drink.icon;
        }

        if (backgroundImage != null && drink != null)
        {
            backgroundImage.color = drink.drinkColor;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
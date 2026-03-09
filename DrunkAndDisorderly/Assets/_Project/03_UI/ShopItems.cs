using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [Header("UI References")]
    public Text nameText;
    public Text priceText;
    public Image iconImage;
    public Button buyButton;

    private string itemName;
    private int price;
    private UpgradeType upgradeType;
    private int upgradeValue;

    private void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(TryBuy);
    }

    public void Initialize(string name, int itemPrice, UpgradeType type, int value)
    {
        itemName = name;
        price = itemPrice;
        upgradeType = type;
        upgradeValue = value;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (nameText != null)
            nameText.text = itemName;

        if (priceText != null)
            priceText.text = price.ToString();
    }

    private void TryBuy()
    {
        if (GoldManager.Instance != null && GoldManager.Instance.SpendGold(price))
        {
            ApplyUpgrade();
            Debug.Log($"Куплено: {itemName}");

            // Делаем товар недоступным для повторной покупки
            if (buyButton != null)
                buyButton.interactable = false;
        }
        else
        {
            Debug.Log("Недостаточно золота!");
        }
    }

    private void ApplyUpgrade()
    {
        switch (upgradeType)
        {
            case UpgradeType.TapSpeed:
                // TODO: увеличить скорость кранов
                Debug.Log($"Увеличение скорости кранов на {upgradeValue}");
                break;

            case UpgradeType.CustomerPatience:
                // TODO: увеличить терпение посетителей
                Debug.Log($"Увеличение терпения на {upgradeValue}");
                break;

            case UpgradeType.DrinkPrice:
                // TODO: увеличить цену напитков
                Debug.Log($"Увеличение цен на {upgradeValue}%");
                break;

            case UpgradeType.NewDrink:
                // TODO: разблокировать новый напиток
                Debug.Log("Разблокирован новый напиток");
                break;
        }
    }
}

public enum UpgradeType
{
    TapSpeed,
    CustomerPatience,
    DrinkPrice,
    NewDrink
}
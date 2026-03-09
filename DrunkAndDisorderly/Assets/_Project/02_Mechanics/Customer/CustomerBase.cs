using UnityEngine;

public class CustomerBase : MonoBehaviour
{
    [Header("Data")]
    public RaceData race;
    public DrinkData currentOrder;

    [Header("State")]
    public float currentPatience;
    public bool isSitting = false;
    public bool isAngry = false;
    public bool isLeaving = false;

    [Header("References")]
    public Transform orderIconPosition;
    public Transform patienceBarPosition;

    private CustomerMovement movement;
    private CustomerEffects effects;
    private CustomerOrder order;
    private CustomerPatience patience;

    private void Awake()
    {
        movement = GetComponent<CustomerMovement>();
        effects = GetComponent<CustomerEffects>();
        order = GetComponent<CustomerOrder>();
        patience = GetComponent<CustomerPatience>();
    }

    public void Initialize(RaceData raceData)
    {
        race = raceData;
        currentPatience = raceData.basePatience;

        Debug.Log($"Customer initialized with race: {raceData.raceName}");
    }

    public void ServeDrink(DrinkData drink)
    {
        if (drink == null) return;

        CustomerOrder order = GetComponent<CustomerOrder>();

        if (order != null && order.IsCorrectDrink(drink))
        {
            Debug.Log($"✅ Правильный напиток! {race?.raceName} получил {drink.drinkName}");
            order.HideOrderIcon();

            // Активируем эффекты напитка
            CustomerEffects effects = GetComponent<CustomerEffects>();
            if (effects != null && drink.specialEffects != null)
            {
                foreach (var effect in drink.specialEffects)
                {
                    if (effect.targetRace == race)
                    {
                        effects.ActivateEffect(effect);
                        break;
                    }
                }
            }

            // Добавляем золото за обслуживание
            GoldManager.Instance?.AddGold(drink.price);

            // Посетитель доволен и уходит
            StartLeaving(true);
        }
        else
        {
            Debug.Log("❌ Неправильный напиток!");
            // Штраф: терпение уменьшается сильнее
            CustomerPatience patience = GetComponent<CustomerPatience>();
            if (patience != null)
            {
                patience.ReducePatience(3f); // штраф 3 секунды
            }
        }
    }

    public void GetAngry()
    {
        if (isAngry) return;

        isAngry = true;
        Debug.Log($"{race?.raceName} разозлился!");

        // TODO: удар по бармену
        // GameManager.Instance?.TakeDamage();

        StartLeaving(false);
    }

    public void StartLeaving(bool happy)
    {
        if (isLeaving) return;

        isLeaving = true;

        // TODO: освободить слот

        if (movement != null)
            movement.StartLeaving();
    }
}
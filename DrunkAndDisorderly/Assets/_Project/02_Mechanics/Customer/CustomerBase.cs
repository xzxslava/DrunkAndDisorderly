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
    public Transform orderIconPosition; // точка для иконки заказа
    public Transform patienceBarPosition; // точка для полоски терпения

    private BarSlot assignedSlot;
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

    public void Initialize(RaceData raceData, BarSlot slot)
    {
        race = raceData;
        assignedSlot = slot;

        currentPatience = race.basePatience;

        movement?.Initialize(slot.transform.position);
        order?.GenerateOrder();
        patience?.Initialize(race.basePatience, race.patienceRate);

        EventManager.OnCustomerSpawned?.Invoke(gameObject);
    }

    public void ServeDrink(DrinkData drink)
    {
        if (drink == currentOrder)
        {
            // Правильный напиток
            bool hasEffect = false;
            foreach (var effect in drink.specialEffects)
            {
                if (effect.targetRace == race)
                {
                    effects?.ActivateEffect(effect);
                    hasEffect = true;
                    break;
                }
            }

            if (!hasEffect)
            {
                // Обычное обслуживание
                GoldManager.Instance?.AddGold(drink.price);
                EventManager.OnCustomerServed?.Invoke(gameObject);
            }

            StartLeaving(true);
        }
        else
        {
            // Неправильный напиток
            Debug.Log("Wrong drink!");
            // Можно добавить штраф
        }
    }

    public void GetAngry()
    {
        if (isAngry) return;

        isAngry = true;
        EventManager.OnCustomerAngry?.Invoke(gameObject);

        // Нанести урон бармену
        GameManager.Instance?.TakeDamage();

        StartLeaving(false);
    }

    public void StartLeaving(bool happy)
    {
        if (isLeaving) return;

        isLeaving = true;

        if (assignedSlot != null)
        {
            assignedSlot.Vacate();
        }

        movement?.StartLeaving();

        if (!happy)
        {
            // Злой уходит быстрее
            movement?.SetSpeed(race.moveSpeed * 1.5f);
        }
    }

    private void OnDestroy()
    {
        EventManager.OnCustomerLeft?.Invoke(gameObject);
    }
}
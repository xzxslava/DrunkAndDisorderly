using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] customerPrefabs; // массив разных типов посетителей

    [Header("Spawn Points")]
    public Transform spawnPoint; // где появляются
    public Transform exitPoint; // куда уходят (не используется напрямую, но для ссылки)

    [Header("Bar Slots")]
    public BarSlot[] barSlots; // все места у бара

    [Header("Settings")]
    public float spawnInterval = 3f; // каждые 3 секунды
    public int maxCustomers = 4; // максимум посетителей одновременно

    [Header("State")]
    public List<GameObject> activeCustomers = new List<GameObject>(); // кто сейчас в баре

    private float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Пробуем создать нового посетителя
            TrySpawnCustomer();

            // Сбрасываем таймер с небольшим разбросом (чтобы не было совсем одинаково)
            timer = spawnInterval + Random.Range(-0.5f, 0.5f);
        }
    }

    void TrySpawnCustomer()
    {
        // Проверяем, есть ли свободные места
        BarSlot freeSlot = GetFreeSlot();

        if (freeSlot == null)
        {
            Debug.Log("Нет свободных мест");
            return;
        }

        // Дополнительная проверка: убедимся что слот действительно свободен
        if (freeSlot.isOccupied)
        {
            Debug.LogWarning($"Слот {freeSlot.slotIndex} помечен как свободный, но isOccupied = true! Ошибка в логике.");
            return;
        }

        if (activeCustomers.Count < maxCustomers)
        {
            SpawnCustomer(freeSlot);
        }
    }

    BarSlot GetFreeSlot()
    {
        foreach (BarSlot slot in barSlots)
        {
            if (!slot.isOccupied)
                return slot;
        }
        return null;
    }

    void SpawnCustomer(BarSlot targetSlot)
    {
        if (customerPrefabs.Length == 0 || spawnPoint == null || targetSlot == null)
        {
            Debug.LogError("Не все ссылки настроены в Spawner!");
            return;
        }

        // ВАЖНО: Помечаем слот как занятый СРАЗУ, чтобы другие не шли сюда
        targetSlot.isOccupied = true;

        // Выбираем случайного посетителя
        int randomIndex = Random.Range(0, customerPrefabs.Length);
        GameObject prefab = customerPrefabs[randomIndex];

        // Создаем посетителя
        GameObject newCustomer = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Настраиваем его движение
        CustomerMovement movement = newCustomer.GetComponent<CustomerMovement>();
        if (movement != null)
        {
            movement.targetSlot = targetSlot;
        }

        // Добавляем в список активных
        activeCustomers.Add(newCustomer);

        Debug.Log($"Создан посетитель {randomIndex} для места {targetSlot.slotIndex}. Место занято.");
    }

    // Метод для удаления посетителя из списка (вызывать из CustomerMovement при уничтожении)
    public void RemoveCustomer(GameObject customer)
    {
        if (activeCustomers.Contains(customer))
        {
            activeCustomers.Remove(customer);
        }
    }
}
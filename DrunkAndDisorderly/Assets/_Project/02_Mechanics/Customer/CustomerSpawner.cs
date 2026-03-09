using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform spawnPoint;
    public List<BarSlot> barSlots = new List<BarSlot>();

    [Header("Prefabs")]
    public GameObject[] customerPrefabs;

    [Header("Settings")]
    public float spawnInterval = 3f;
    public int maxCustomers = 8;
    public bool spawnOnStart = true;

    [Header("State")]
    public List<GameObject> activeCustomers = new List<GameObject>();

    private float timer;

    private void Start()
    {
        if (spawnOnStart)
        {
            StartSpawning();
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && activeCustomers.Count < maxCustomers)
        {
            TrySpawnCustomer();
            timer = spawnInterval;
        }
    }

    public void StartSpawning()
    {
        timer = spawnInterval;
    }

    private void TrySpawnCustomer()
    {
        BarSlot freeSlot = GetFreeSlot();

        if (freeSlot != null)
        {
            SpawnCustomer(freeSlot);
        }
    }

    private BarSlot GetFreeSlot()
    {
        foreach (BarSlot slot in barSlots)
        {
            if (slot != null && !slot.isOccupied)
                return slot;
        }
        return null;
    }

    private void SpawnCustomer(BarSlot targetSlot)
    {
        if (customerPrefabs.Length == 0 || spawnPoint == null || targetSlot == null)
        {
            Debug.LogError("Spawner not properly configured!");
            return;
        }

        // Помечаем слот как занятый сразу
        targetSlot.isOccupied = true;

        // Выбираем случайного посетителя
        GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];

        // Создаём посетителя
        GameObject newCustomer = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        activeCustomers.Add(newCustomer);

        // ИСПРАВЛЕНИЕ: используем Initialize вместо прямого доступа к targetSlot
        CustomerMovement movement = newCustomer.GetComponent<CustomerMovement>();
        if (movement != null)
        {
            // Передаём позицию слота через Initialize
            movement.Initialize(targetSlot.transform.position);
        }

        // Сохраняем ссылку на слот в самом посетителе (если нужно)
        CustomerBase customerBase = newCustomer.GetComponent<CustomerBase>();
        if (customerBase != null)
        {
            // TODO: инициализация с расой
        }

        Debug.Log($"Customer spawned for slot {barSlots.IndexOf(targetSlot)}");
    }

    public void RemoveCustomer(GameObject customer)
    {
        if (activeCustomers.Contains(customer))
        {
            activeCustomers.Remove(customer);
        }
    }
}
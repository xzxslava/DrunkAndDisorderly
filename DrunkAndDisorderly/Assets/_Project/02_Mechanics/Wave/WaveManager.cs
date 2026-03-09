using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public Transform spawnPoint;
    public Transform exitPoint;
    public List<BarSlot> barSlots = new List<BarSlot>();

    [Header("Prefabs")]
    public GameObject[] customerPrefabs;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public int wavesPerDay = 3;
    public float initialDelay = 2f;
    public float waveInterval = 10f;
    public bool autoStartNextWave = true;

    [Header("State")]
    public bool isWaveActive = false;
    public int activeCustomers = 0;

    private DayProgression dayProgression;
    private Coroutine waveCoroutine;
    private GoldManager goldManager;
    private GameManager gameManager;

    private void Start()
    {
        Debug.Log("WaveManager Start");

        // Находим необходимые компоненты
        dayProgression = GetComponent<DayProgression>();
        if (dayProgression == null)
            dayProgression = FindObjectOfType<DayProgression>();

        goldManager = GetComponent<GoldManager>();
        if (goldManager == null)
            goldManager = FindObjectOfType<GoldManager>();

        gameManager = GetComponent<GameManager>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        // Проверка настроек
        Debug.Log($"SpawnPoint: {spawnPoint}");
        Debug.Log($"ExitPoint: {exitPoint}");
        Debug.Log($"BarSlots count: {(barSlots != null ? barSlots.Count : 0)}");
        Debug.Log($"CustomerPrefabs count: {(customerPrefabs != null ? customerPrefabs.Length : 0)}");

        // Проверка каждого слота
        if (barSlots != null)
        {
            for (int i = 0; i < barSlots.Count; i++)
            {
                Debug.Log($"BarSlot[{i}]: {barSlots[i]}");
            }
        }
    }

    public void StartWaves()
    {
        Debug.Log("StartWaves called");

        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogError("No customer prefabs assigned!");
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint not assigned!");
            return;
        }

        if (barSlots == null || barSlots.Count == 0)
        {
            Debug.LogError("No bar slots assigned!");
            return;
        }

        if (waveCoroutine != null)
            StopCoroutine(waveCoroutine);

        waveCoroutine = StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        Debug.Log("WaveRoutine started");
        yield return new WaitForSeconds(initialDelay);

        for (currentWave = 0; currentWave < wavesPerDay; currentWave++)
        {
            Debug.Log($"Starting wave {currentWave + 1} of {wavesPerDay}");
            StartWave(currentWave);

            // Ждём, пока волна закончится (все посетители обслужены/ушли)
            while (activeCustomers > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log($"Wave {currentWave + 1} completed. Active customers: {activeCustomers}");

            if (currentWave < wavesPerDay - 1)
            {
                // Небольшая пауза между волнами
                Debug.Log($"Waiting {waveInterval} seconds before next wave");
                yield return new WaitForSeconds(waveInterval);
            }
        }

        Debug.Log("All waves completed for today");

        if (gameManager != null)
            gameManager.EndDay();
    }

    private void StartWave(int waveIndex)
    {
        isWaveActive = true;
        activeCustomers = 0; // Сбрасываем счётчик для новой волны
        EventManager.OnWaveStarted?.Invoke(waveIndex);

        int customersCount = dayProgression != null
            ? dayProgression.GetCustomersPerWave()
            : 3 + waveIndex;

        Debug.Log($"Wave {waveIndex + 1} will spawn {customersCount} customers");
        StartCoroutine(SpawnWaveCustomers(customersCount));
    }

    private IEnumerator SpawnWaveCustomers(int count)
    {
        Debug.Log($"Starting to spawn {count} customers");

        for (int i = 0; i < count; i++)
        {
            SpawnCustomer();
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    private void SpawnCustomer()
    {
        Debug.Log("Trying to spawn customer");

        // Проверяем наличие свободных слотов
        BarSlot freeSlot = GetFreeSlot();
        if (freeSlot == null)
        {
            Debug.Log("No free slots available");
            return;
        }

        Debug.Log($"Free slot found at index {barSlots.IndexOf(freeSlot)}");

        // Проверяем префабы
        if (customerPrefabs == null || customerPrefabs.Length == 0)
        {
            Debug.LogError("No customer prefabs assigned!");
            return;
        }

        // Выбираем случайного посетителя
        int randomIndex = Random.Range(0, customerPrefabs.Length);
        GameObject prefab = customerPrefabs[randomIndex];

        if (prefab == null)
        {
            Debug.LogError($"Prefab at index {randomIndex} is null!");
            return;
        }

        Debug.Log($"Selected prefab: {prefab.name}");

        // Помечаем слот как занятый сразу
        freeSlot.isOccupied = true;

        // Создаём посетителя
        GameObject newCustomer = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        activeCustomers++;

        Debug.Log($"Customer instantiated. Active customers: {activeCustomers}");

        // Настраиваем движение
        CustomerMovement movement = newCustomer.GetComponent<CustomerMovement>();
        if (movement != null)
        {
            movement.Initialize(freeSlot.transform.position);
            movement.SetExitPoint(exitPoint); // Добавим этот метод позже
            Debug.Log("Movement initialized");
        }
        else
        {
            Debug.LogError("Customer prefab has no CustomerMovement component!");
        }

        // Настраиваем слот
        freeSlot.Occupy(newCustomer);

        // Подписываемся на событие ухода
        StartCoroutine(WaitForCustomerLeave(newCustomer));
    }

    private IEnumerator WaitForCustomerLeave(GameObject customer)
    {
        // Ждём, пока объект не будет уничтожен
        yield return new WaitUntil(() => customer == null);
        CustomerLeft();
    }

    private BarSlot GetFreeSlot()
    {
        if (barSlots == null) return null;

        foreach (var slot in barSlots)
        {
            if (slot != null && !slot.isOccupied)
                return slot;
        }
        return null;
    }

    public void CustomerLeft()
    {
        activeCustomers--;
        Debug.Log($"Customer left. Active customers: {activeCustomers}");

        if (activeCustomers <= 0 && isWaveActive)
        {
            isWaveActive = false;
            EventManager.OnWaveEnded?.Invoke();
            Debug.Log("Wave ended");
        }
    }

    public Transform GetExitPoint()
    {
        return exitPoint;
    }

    // Визуализация в редакторе
    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPoint.position, 0.3f);
            Gizmos.DrawWireCube(spawnPoint.position + Vector3.up, new Vector3(1, 2, 1));
        }

        if (exitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(exitPoint.position, 0.3f);
            Gizmos.DrawWireCube(exitPoint.position + Vector3.up, new Vector3(1, 2, 1));
        }
    }
}
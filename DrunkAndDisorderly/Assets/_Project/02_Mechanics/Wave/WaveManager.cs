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
    public GameObject[] customerPrefabs; // префабы разных рас

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

    private void Awake()
    {
        dayProgression = GetComponent<DayProgression>();
        if (dayProgression == null)
            dayProgression = FindObjectOfType<DayProgression>();
    }

    public void StartWaves()
    {
        if (waveCoroutine != null)
            StopCoroutine(waveCoroutine);

        waveCoroutine = StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        yield return new WaitForSeconds(initialDelay);

        for (currentWave = 0; currentWave < wavesPerDay; currentWave++)
        {
            StartWave(currentWave);

            // Ждём, пока волна закончится (все посетители обслужены/ушли)
            while (activeCustomers > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

            Debug.Log($"Wave {currentWave + 1} completed");

            if (currentWave < wavesPerDay - 1)
            {
                // Небольшая пауза между волнами
                yield return new WaitForSeconds(2f);

                // Автоматический запуск или ожидание тапа игрока
                if (autoStartNextWave)
                {
                    // Продолжаем автоматически
                }
                else
                {
                    // TODO: ждём тапа игрока по кнопке
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
                }
            }
        }

        Debug.Log("All waves completed for today");
        GameManager.Instance?.EndDay();
    }

    private void StartWave(int waveIndex)
    {
        isWaveActive = true;
        EventManager.OnWaveStarted?.Invoke(waveIndex);

        int customersCount = dayProgression != null
            ? dayProgression.GetCustomersPerWave()
            : 3 + waveIndex;

        StartCoroutine(SpawnWaveCustomers(customersCount));
    }

    private IEnumerator SpawnWaveCustomers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnCustomer();
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    private void SpawnCustomer()
    {
        // Находим свободный слот
        BarSlot freeSlot = GetFreeSlot();
        if (freeSlot == null)
        {
            Debug.Log("No free slots, customer will wait");
            // TODO: реализовать ожидание у входа
            return;
        }

        // Выбираем случайного посетителя
        GameObject prefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];

        // Спавним через пул или напрямую
        GameObject customer = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        activeCustomers++;

        // Инициализируем
        CustomerBase customerBase = customer.GetComponent<CustomerBase>();
        if (customerBase != null)
        {
            // TODO: передавать данные о расе
            customerBase.Initialize(null, freeSlot);
        }
        else
        {
            // Если нет CustomerBase, хотя бы двигаем к слоту
            CustomerMovement movement = customer.GetComponent<CustomerMovement>();
            if (movement != null)
            {
                movement.Initialize(freeSlot.transform.position);
            }
        }

        freeSlot.Occupy(customer);
    }

    private BarSlot GetFreeSlot()
    {
        foreach (var slot in barSlots)
        {
            if (!slot.isOccupied)
                return slot;
        }
        return null;
    }

    public void CustomerLeft()
    {
        activeCustomers--;
        if (activeCustomers <= 0 && isWaveActive)
        {
            isWaveActive = false;
            EventManager.OnWaveEnded?.Invoke();
        }
    }

    public Transform GetExitPoint()
    {
        return exitPoint;
    }
}
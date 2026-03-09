using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [System.Serializable]
    public class Resource
    {
        public string resourceName;
        public int amount;
        public int maxAmount = 50;
        public Sprite icon;
    }

    [Header("Resources")]
    public List<Resource> resources = new List<Resource>();

    [Header("Delivery")]
    public int deliveryTime = 30; // секунд
    public bool isDeliveryPending = false;
    private float deliveryTimer;

    private Dictionary<string, Resource> resourceDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        resourceDict = new Dictionary<string, Resource>();
        foreach (var res in resources)
        {
            resourceDict[res.resourceName] = res;
        }
    }

    private void Update()
    {
        if (isDeliveryPending)
        {
            deliveryTimer -= Time.deltaTime;

            if (deliveryTimer <= 0)
            {
                CompleteDelivery();
            }
        }
    }

    public int GetResourceAmount(string resourceName)
    {
        if (resourceDict.ContainsKey(resourceName))
            return resourceDict[resourceName].amount;
        return 0;
    }

    public bool ConsumeResource(string resourceName, int amount)
    {
        if (!resourceDict.ContainsKey(resourceName))
            return false;

        if (resourceDict[resourceName].amount < amount)
            return false;

        resourceDict[resourceName].amount -= amount;
        Debug.Log($"Consumed {amount} {resourceName}. Left: {resourceDict[resourceName].amount}");
        return true;
    }

    public void OrderResource(string resourceName, int amount)
    {
        if (!resourceDict.ContainsKey(resourceName))
            return;

        if (isDeliveryPending)
        {
            Debug.Log("Delivery already pending");
            return;
        }

        // TODO: проверить достаточно ли золота
        int cost = amount * 5; // базова€ цена

        if (GoldManager.Instance.SpendGold(cost))
        {
            isDeliveryPending = true;
            deliveryTimer = deliveryTime;

            // «апоминаем заказ
            Debug.Log($"Ordered {amount} {resourceName}. Delivery in {deliveryTime}s");

            // «апускаем корутину или таймер
            StartCoroutine(DeliveryCoroutine(resourceName, amount));
        }
    }

    private System.Collections.IEnumerator DeliveryCoroutine(string resourceName, int amount)
    {
        yield return new WaitForSeconds(deliveryTime);

        if (resourceDict.ContainsKey(resourceName))
        {
            resourceDict[resourceName].amount += amount;
            Debug.Log($"Delivered {amount} {resourceName}. Total: {resourceDict[resourceName].amount}");
        }

        isDeliveryPending = false;
    }

    private void CompleteDelivery()
    {
        isDeliveryPending = false;
        // TODO: добавить ресурсы
    }

    public void AddResource(string resourceName, int amount)
    {
        if (resourceDict.ContainsKey(resourceName))
        {
            resourceDict[resourceName].amount += amount;
        }
    }
}
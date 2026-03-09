using UnityEngine;

public class BarSlot : MonoBehaviour
{
    [Header("Settings")]
    public int slotIndex;

    [Header("State")]
    public bool isOccupied = false;
    public GameObject currentCustomer;

    private void Start()
    {
        // Визуальная идентификация в редакторе
        gameObject.name = $"BarSlot_{slotIndex}";
    }

    public void Occupy(GameObject customer)
    {
        isOccupied = true;
        currentCustomer = customer;
    }

    public void Vacate()
    {
        isOccupied = false;
        currentCustomer = null;
    }

    private void OnDrawGizmos()
    {
        if (isOccupied)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(0.5f, 1f, 0.5f));
    }
}
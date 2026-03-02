using UnityEngine;

public class BarSlot : MonoBehaviour
{
    [Header("Settings")]
    public int slotIndex; // номер места (0,1,2,3)

    [Header("State")]
    public bool isOccupied = false; // занято ли место
    public GameObject currentCustomer; // кто сейчас сидит

    // Метод для занятия места
    public void Occupy(GameObject customer)
    {
        isOccupied = true;
        currentCustomer = customer;
    }

    // Метод для освобождения места
    public void Vacate()
    {
        isOccupied = false;
        currentCustomer = null;
    }

    // Визуализация в редакторе (чтобы видеть состояние)
    private void OnDrawGizmos()
    {
        if (isOccupied)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.5f, new Vector3(0.5f, 1f, 0.5f));
    }
}
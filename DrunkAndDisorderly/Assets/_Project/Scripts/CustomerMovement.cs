using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;

    [Header("References")]
    public BarSlot targetSlot; // к какому месту идет

    [Header("State")]
    public bool isMovingToSlot = true; // идет к месту или уже сидит
    public bool isLeaving = false; // уходит ли из бара

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        // Сразу начинаем двигаться к месту
        if (targetSlot != null)
        {
            targetPosition = targetSlot.transform.position;

            // Поворачиваемся лицом к бару (условно)
            Vector3 directionToBar = new Vector3(-1, 0, 0); // смотрим налево (где бар)
            targetRotation = Quaternion.LookRotation(directionToBar);
        }
    }

    void Update()
    {
        if (isLeaving)
        {
            // Двигаемся к выходу
            MoveToExit();
        }
        else if (isMovingToSlot)
        {
            // Двигаемся к месту
            MoveToSlot();
        }
    }

    void MoveToSlot()
    {
        if (targetSlot == null) return;

        // Двигаемся к позиции места
        Vector3 slotPos = targetSlot.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, slotPos, moveSpeed * Time.deltaTime);

        // Плавно поворачиваемся
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Проверяем, дошли ли
        if (Vector3.Distance(transform.position, slotPos) < 0.1f)
        {
            // Дошли - останавливаемся
            isMovingToSlot = false;
            transform.position = slotPos; // фиксируем позицию

            // Уже не нужно занимать место - оно уже занято с момента спавна
            // Но можно вызвать для синхронизации currentCustomer
            targetSlot.Occupy(gameObject);

            Debug.Log($"Посетитель сел на место {targetSlot.slotIndex}");
        }
    }

    void MoveToExit()
    {
        // Находим выход (временно жестко закодирован)
        Vector3 exitPos = new Vector3(5, 0, -3);

        transform.position = Vector3.MoveTowards(transform.position, exitPos, moveSpeed * Time.deltaTime);

        // Если дошел до выхода - удаляем
        if (Vector3.Distance(transform.position, exitPos) < 0.5f)
        {
            // Освобождаем место, если оно было занято
            if (targetSlot != null)
                targetSlot.Vacate();

            Destroy(gameObject);
            Debug.Log("Посетитель ушел");
        }
    }

    // Метод для начала ухода
    public void StartLeaving()
    {
        isLeaving = true;
        isMovingToSlot = false;

        // Освобождаем место
        if (targetSlot != null)
            targetSlot.Vacate();
    }

    // Визуализация пути в редакторе
    private void OnDrawGizmos()
    {
        if (targetSlot != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetSlot.transform.position);
        }
    }
}
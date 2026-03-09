using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;

    [Header("State")]
    public bool isMoving = true;
    public bool isLeaving = false;
    public bool isInitialized = false; // Флаг инициализации

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Transform exitPoint;
    private CustomerBase customer;

    private void Awake()
    {
        customer = GetComponent<CustomerBase>();
    }

    public void Initialize(Vector3 slotPosition)
    {
        targetPosition = slotPosition;

        // Поворот лицом к бару (условно)
        Vector3 directionToBar = new Vector3(-1, 0, 0);
        if (directionToBar != Vector3.zero)
        {
            targetRotation = Quaternion.LookRotation(directionToBar);
        }
        else
        {
            targetRotation = Quaternion.identity;
        }

        isMoving = true;
        isInitialized = true;

        Debug.Log($"Customer initialized. Target: {targetPosition}");
    }

    public void SetExitPoint(Transform exit)
    {
        exitPoint = exit;
        Debug.Log($"Exit point set: {exit.position}");
    }

    private void Update()
    {
        // Не двигаемся, если не инициализированы
        if (!isInitialized) return;

        if (isLeaving)
        {
            MoveToExit();
        }
        else if (isMoving)
        {
            MoveToSlot();
        }
    }

    private void MoveToSlot()
    {
        // Проверка на валидность
        if (targetPosition == null)
        {
            Debug.LogError("Target position is null!");
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            if (customer != null)
                customer.isSitting = true;

            Debug.Log($"{name} сел на место");
        }
    }

    private void MoveToExit()
    {
        if (exitPoint == null)
        {
            Debug.LogError("Exit point is null, destroying customer");
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, exitPoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, exitPoint.position) < 0.5f)
        {
            Destroy(gameObject);
        }
    }

    public void StartLeaving()
    {
        isLeaving = true;
        isMoving = false;
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
}
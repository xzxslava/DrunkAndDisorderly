using UnityEngine;

public class CustomerMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;

    [Header("State")]
    public bool isMoving = true;
    public bool isLeaving = false;

    private Vector3 targetPosition;
    private Quaternion targetRotation;
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
        targetRotation = Quaternion.LookRotation(directionToBar);

        isMoving = true;
    }

    private void Update()
    {
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
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            customer.isSitting = true;

            // Сидим на месте, ждём
            Debug.Log($"{name} сел на место");
        }
    }

    private void MoveToExit()
    {
        Vector3 exitPos = new Vector3(5, 0, -3); // TODO: брать из GameManager или WaveManager

        transform.position = Vector3.MoveTowards(transform.position, exitPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, exitPos) < 0.5f)
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
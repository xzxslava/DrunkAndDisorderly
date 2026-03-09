using UnityEngine;

public class CustomerPatience : MonoBehaviour
{
    [Header("Settings")]
    public float maxPatience = 10f;
    public float patienceRate = 1f;

    [Header("UI")]
    public GameObject patienceBarPrefab;
    public Transform barPosition;

    private float currentPatience;
    private bool isWaiting = true;
    private GameObject activeBar;
    private UnityEngine.UI.Slider barSlider;
    private CustomerBase customer;

    private void Awake()
    {
        customer = GetComponent<CustomerBase>();
        if (barPosition == null)
            barPosition = transform;
    }

    public void Initialize(float maxPat, float rate)
    {
        maxPatience = maxPat;
        patienceRate = rate;
        currentPatience = maxPatience;
        isWaiting = true;

        CreatePatienceBar();
    }

    private void Update()
    {
        if (!isWaiting || customer.isLeaving) return;

        currentPatience -= Time.deltaTime * patienceRate;

        if (barSlider != null)
        {
            barSlider.value = currentPatience / maxPatience;
        }

        if (currentPatience <= 0)
        {
            isWaiting = false;
            customer.GetAngry();
            DestroyPatienceBar();
        }
    }

    private void CreatePatienceBar()
    {
        if (patienceBarPrefab == null) return;

        activeBar = Instantiate(patienceBarPrefab, barPosition.position, Quaternion.identity, transform);
        barSlider = activeBar.GetComponent<UnityEngine.UI.Slider>();

        if (barSlider != null)
        {
            barSlider.maxValue = 1f;
            barSlider.value = 1f;
        }

        // Добавляем Billboard для поворота к камере
        if (activeBar.GetComponent<Billboard>() == null)
        {
            activeBar.AddComponent<Billboard>();
        }
    }

    private void DestroyPatienceBar()
    {
        if (activeBar != null)
        {
            Destroy(activeBar);
        }
    }

    public void ResetPatience()
    {
        currentPatience = maxPatience;
        if (barSlider != null)
            barSlider.value = 1f;
    }
}
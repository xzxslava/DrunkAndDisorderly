using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool reverse = false;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera == null) return;

        // Поворачиваем объект лицом к камере
        transform.LookAt(transform.position + mainCamera.transform.forward);

        if (reverse)
        {
            transform.Rotate(0, 180, 0);
        }
    }
}
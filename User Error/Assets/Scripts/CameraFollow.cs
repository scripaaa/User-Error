using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Основные настройки")]
    public Transform target;                // Игрок
    public Vector3 offset = new Vector3(0, 1, -10);
    public float smoothSpeed = 0.15f;

    [Header("Граница камеры (BoxCollider2D)")]
    public BoxCollider2D cameraBounds;

    private float minX, maxX, minY, maxY;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("CameraFollow: Main Camera not found!");
            return;
        }

        if (cameraBounds == null)
        {
            Debug.LogError("CameraFollow: CameraBounds is not assigned!");
            return;
        }

        // Размеры камеры
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // Границы BoxCollider2D
        Bounds bounds = cameraBounds.bounds;

        // Вычисляем реальные границы, учитывая размер камеры
        minX = bounds.min.x + camWidth;
        maxX = bounds.max.x - camWidth;
        minY = bounds.min.y + camHeight;
        maxY = bounds.max.y - camHeight;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Желаемая позиция камеры
        Vector3 desiredPosition = target.position + offset;

        // Ограничиваем камеру границами
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        // Плавное движение
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}

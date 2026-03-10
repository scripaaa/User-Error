using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [Header("Reference Resolution")]
    public float referenceWidth = 1920f;
    public float referenceHeight = 1080f;

    [Header("Base Orthographic Size")]
    public float baseOrthoSize = 5f;

    private Camera cam;
    private int lastWidth;
    private int lastHeight;

    void Awake()
    {
        cam = GetComponent<Camera>();
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        UpdateCameraSize();
        StartCoroutine(UpdateAfterFrame());
    }

    IEnumerator UpdateAfterFrame()
    {
        yield return null; // ждЄм один кадр Ч экран точно инициализирован
        UpdateCameraSize();
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;
            UpdateCameraSize();
        }
    }

    void UpdateCameraSize()
    {
        float currentAspect = (float)Screen.width / Screen.height;
        float referenceAspect = referenceWidth / referenceHeight;

        if (currentAspect >= referenceAspect)
        {
            // Ёкран шире или равен Ч масштабируем по высоте
            cam.orthographicSize = baseOrthoSize;
        }
        else
        {
            // Ёкран уже Ч увеличиваем чтобы всЄ влезло по ширине
            cam.orthographicSize = baseOrthoSize * (referenceAspect / currentAspect);
        }

        Debug.Log($"[CameraScaler] {Screen.width}x{Screen.height} | aspect: {currentAspect:F2} | orthoSize: {cam.orthographicSize:F2}");
    }
}
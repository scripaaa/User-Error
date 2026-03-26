using UnityEngine;
using TMPro;

public class TriggerPrompt : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private GameObject promptContainer; // Объект с текстом (Canvas или сам Текст)

    [Header("Настройки позиции")]
    [SerializeField] private float verticalOffset = 1.5f;
    [SerializeField] private bool isInverted = false;

    void Start()
    {
        // Скрываем подсказку при запуске
        if (promptContainer != null)
        {
            UpdatePosition();
            promptContainer.SetActive(false);
        }
    }

    // Срабатывает, когда объект с тегом Player входит в зону
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UpdatePosition(); // Обновляем позицию перед показом
            promptContainer.SetActive(true);
        }
    }

    // Срабатывает, когда игрок выходит из зоны
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            promptContainer.SetActive(false);
        }
    }

    // Тот же метод для управления "верх/низ"
    private void UpdatePosition()
    {
        if (promptContainer == null) return;

        // Если inverted = true, текст сверху (offset). Если false, текст снизу (-offset).
        float currentOffset = isInverted ? verticalOffset : -verticalOffset;
        promptContainer.transform.localPosition = new Vector3(0, currentOffset, 0);

        // Разворачиваем текст, чтобы он не был вверх ногами в инвертированном режиме
        if (isInverted)
            promptContainer.transform.localRotation = Quaternion.Euler(0, 0, 180f);
        else
            promptContainer.transform.localRotation = Quaternion.identity;
    }
}
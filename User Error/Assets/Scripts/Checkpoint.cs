using UnityEngine;

/// <summary>
/// Компонент, который делает объект точкой возрождения.
/// Требуется Collider2D, отмеченный как Trigger.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    // Ссылка на наш уровень‑специфичный менеджер
    private LevelCheckpointManager _checkpointManager;

    private void Awake()
    {
        // Находим менеджер в сцене один раз
        _checkpointManager = FindObjectOfType<LevelCheckpointManager>();
        if (_checkpointManager == null)
            Debug.LogError("[Checkpoint] LevelCheckpointManager не найден в сцене!");
    }

    // Срабатывает, когда в триггер попадает любой коллайдер
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Мы реагируем только на игрока
        if (!other.CompareTag("Player")) return;

        // Устанавливаем новую позицию чекпоинта
        _checkpointManager.SetCheckpoint(transform.position);

        // (Необязательно) Визуальный/звуковой отклик
        // Debug.Log($"[Checkpoint] Активирован чекпоинт в {transform.position}");
    }

#if UNITY_EDITOR
    // Чтобы в редакторе видеть область триггера
    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }
#endif
}

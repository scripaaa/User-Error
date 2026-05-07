using UnityEngine;

public class WallDestructor : MonoBehaviour
{
    [Tooltip("Другой объект, который нужно скрыть при разрушении стены")]
    public GameObject objectToHide;

    /// <summary>
    /// Вызовите этот метод, чтобы разрушить стену.
    /// </summary>
    public void DestroyWall()
    {
        // Скрываем указанный объект, если он задан
        if (objectToHide != null)
            objectToHide.SetActive(false);

        // Удаляем саму стену
        Destroy(gameObject);
    }

    // Пример срабатывания при входе в триггер (например, игроком)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            DestroyWall();
    }
}

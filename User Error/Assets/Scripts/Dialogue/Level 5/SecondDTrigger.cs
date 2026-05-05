using UnityEngine;

public class SecondDTrigger : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private GameObject firstTriggerObject; // сюда перетащить объект FirstTrigger

    private SecondController sc;

    void Start()
    {
        sc = FindObjectOfType<SecondController>();
        if (sc == null)
            Debug.LogError("SecondController не найден на сцене!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        // Проверяем, собраны ли все чипы
        if (ChipManager.Instance == null || !ChipManager.Instance.HasAllChips())
        {
            Debug.Log("Не все чипы собраны, триггер не активен.");
            return;
        }

        // Запускаем корутину диалога
        if (sc != null)
        {
            sc.StartCoroutine(sc.CompleteStartSequence());
        }

        // Удаляем объект firstTrigger (если он задан)
        if (firstTriggerObject != null)
        {
            Destroy(firstTriggerObject);
        }
        else
        {
            Debug.LogWarning("firstTriggerObject не назначен в инспекторе! Объект не будет удалён.");
        }

        // Отключаем сам триггер
        gameObject.SetActive(false);
    }
}

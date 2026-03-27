using UnityEngine;

public class Level3DialogueTrigger : MonoBehaviour
{
    private Level3Controller levelController;


    void Start()
    {
        // Находим контроллер уровня
        levelController = FindObjectOfType<Level3Controller>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            levelController.StartCoroutine(levelController.CompleteStartSequence());
            gameObject.SetActive(false); // Отключаем сам триггер
        }
    }
}

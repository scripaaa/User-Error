using UnityEngine;

public class FirstTrigger : MonoBehaviour
{
    private FirstController FirstController;


    void Start()
    {
        // Находим контроллер уровня
        FirstController = FindObjectOfType<FirstController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FirstController.StartCoroutine(FirstController.CompleteStartSequence());
            gameObject.SetActive(false); // Отключаем сам триггер
        }
    }
}
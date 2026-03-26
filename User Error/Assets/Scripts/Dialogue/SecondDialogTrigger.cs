using UnityEngine;

public class SecondDialogTrigger : MonoBehaviour
{
    private SecondDialofController levelController;


    void Start()
    {
        // Находим контроллер уровня
        levelController = FindObjectOfType<SecondDialofController>();
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

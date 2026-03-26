using UnityEngine;

public class FirstDialogTrigger : MonoBehaviour
{
    


    private LevelFirstController levelController;
    

    void Start()
    {
        // Находим контроллер уровня
        levelController = FindObjectOfType<LevelFirstController>();
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

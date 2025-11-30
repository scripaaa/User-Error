using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    // В эту переменную в инспекторе ты впишешь название сцены для загрузки
    [SerializeField] private string sceneToLoad = "PortalScene1";
    
    // В эту переменную в инспекторе ты впишешь имя объекта, где игрок должен появиться
    [SerializeField] private string spawnPointName = "SpawnPoint";

    private bool isPlayerInRange = false;

    void Update()
    {
        // Если игрок рядом и жмет E - грузим сцену
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Сохраняем, куда вернуться, ПЕРЕД тем как уйти
            PlayerPrefs.SetString("SpawnPointName", spawnPointName);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // Когда игрок вошел в зону портала
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    // Когда игрок вышел из зоны портала
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
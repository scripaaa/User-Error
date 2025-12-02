using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "PortalScene1";
    [SerializeField] private string spawnPointName = "SpawnPoint";
    [SerializeField] private bool isOneWayPortal = false; // Если true - только в одну сторону

    private bool isPlayerInRange = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Сохраняем данные для следующей сцены
            GameStateManager.SpawnPointName = spawnPointName;
            GameStateManager.CameFromPortal = true;

            Debug.Log($"Переход через портал в сцену: {sceneToLoad}, точка спавна: {spawnPointName}");

            // Загружаем сцену
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
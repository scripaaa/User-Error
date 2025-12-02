using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform defaultSpawnPoint; // Перетащите стартовую точку в инспекторе

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Игрока с тегом 'Player' на сцене нет! Исправь.");
            return;
        }

        // Если игрок пришел через портал - спавним в точке портала
        if (GameStateManager.CameFromPortal && !string.IsNullOrEmpty(GameStateManager.SpawnPointName))
        {
            GameObject spawnPoint = GameObject.Find(GameStateManager.SpawnPointName);

            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"Игрок спавнится в точке портала: {GameStateManager.SpawnPointName}");
            }
            else
            {
                Debug.LogWarning($"Не найдена точка спавна: {GameStateManager.SpawnPointName}. Используем дефолтную.");
                SpawnAtDefaultPoint(player);
            }

            // Сбрасываем флаг после использования
            GameStateManager.CameFromPortal = false;
        }
        else
        {
            // Иначе спавним в дефолтной точке
            SpawnAtDefaultPoint(player);
        }
    }

    private void SpawnAtDefaultPoint(GameObject player)
    {
        if (defaultSpawnPoint != null)
        {
            player.transform.position = defaultSpawnPoint.position;
            Debug.Log("Игрок спавнится в дефолтной точке");
        }
        else
        {
            Debug.LogWarning("Дефолтная точка спавна не назначена в инспекторе!");
        }
    }
}
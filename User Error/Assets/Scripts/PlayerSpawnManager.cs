using UnityEngine;
using System.Collections;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform defaultSpawnPoint;

    IEnumerator Start()
    {
        // Ждем один кадр, чтобы все компоненты успели инициализироваться
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Игрока с тегом 'Player' на сцене нет! Исправь.");
            yield break;
        }

        Vector3 spawnPosition;

        if (GameStateManager.CameFromPortal && !string.IsNullOrEmpty(GameStateManager.SpawnPointName))
        {
            string spawnPointName = GameStateManager.SpawnPointName;
            GameObject spawnPoint = GameObject.Find(spawnPointName);

            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                spawnPosition = spawnPoint.transform.position;
                Debug.Log($"Игрок спавнится в точке портала: {spawnPointName}");
            }
            else
            {
                spawnPosition = defaultSpawnPoint.position;
                player.transform.position = spawnPosition;
                Debug.LogWarning($"Не найдена точка спавна: {spawnPointName}. Используем дефолтную.");
            }

            GameStateManager.CameFromPortal = false;
        }
        else
        {
            if (defaultSpawnPoint != null)
            {
                spawnPosition = defaultSpawnPoint.position;
                player.transform.position = spawnPosition;
                Debug.Log("Игрок спавнится в дефолтной точке");
            }
            else
            {
                Debug.LogWarning("Дефолтная точка спавна не назначена в инспекторе!");
                yield break;
            }
        }

        // Мгновенно перемещаем камеру в центр комнаты
        SnapCameraToPlayerPosition(spawnPosition);
    }

    private void SnapCameraToPlayerPosition(Vector3 playerPosition)
    {
        RoomManager roomManager = FindObjectOfType<RoomManager>();
        if (roomManager != null)
        {
            roomManager.SnapCameraToPosition(playerPosition);
        }
    }
}
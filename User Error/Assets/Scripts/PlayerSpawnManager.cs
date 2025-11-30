using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Игрока с тегом 'Player' на сцене нет! Исправь.");
            return;
        }

        if (PlayerPrefs.HasKey("SpawnPointName"))
        {
            string spawnPointName = PlayerPrefs.GetString("SpawnPointName");
            GameObject spawnPoint = GameObject.Find(spawnPointName);

            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
            }
        }
    }
}
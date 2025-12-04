using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // Сюда в инспекторе перетащим второй портал, куда телепортируем
    [SerializeField] private Transform destination;

    private bool isPlayerInRange = false;

    void Update()
    {
        // Если игрок рядом и жмет E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Находим игрока на сцене
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Мгновенно меняем его позицию на позицию портала назначения
                player.transform.position = destination.position;
                Debug.Log("Игрок телепортирован в " + destination.name);
            }
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
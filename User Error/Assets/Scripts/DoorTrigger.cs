using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorController door;      // ссылка на дверь
    public GameObject chipCanvas;    // ← сюда вставляешь Canvas

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // включаем Canvas
            if (chipCanvas != null)
                chipCanvas.SetActive(true);

            // запускаем глюк двери
            door.ActivateGlitchWithDelay(0f);
        }
    }
}

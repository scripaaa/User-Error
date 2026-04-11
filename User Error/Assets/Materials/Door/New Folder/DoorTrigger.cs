using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public DoorController door;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            door.ActivateGlitchWithDelay(1f); // задержка 1 секунда
        }
    }
}

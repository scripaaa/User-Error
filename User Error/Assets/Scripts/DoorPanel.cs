using UnityEngine;

public class DoorPanel : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public DoorController door;   // —сылка на дверь в инспекторе

    private bool playerInZone = false;

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(interactKey))
        {
            if (ChipManager.Instance.HasAllChips())
            {
                door.ActivateGlitchWithDelay(0f); // моментальное открытие
            }
            else
            {
                Debug.Log("Ќедостаточно чипов!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInZone = false;
    }
}

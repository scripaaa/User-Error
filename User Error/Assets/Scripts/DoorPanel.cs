using UnityEngine;

public class DoorPanel : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;
    public DoorController door;

    private bool playerInZone = false;
    private TriggerPrompt prompt;

    void Start()
    {
        prompt = GetComponent<TriggerPrompt>();
    }

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(interactKey))
        {
            // Для теста: игнорируем сбор чипов
            // if (true || (ChipManager.Instance != null && ChipManager.Instance.HasAllChips()))
            if (ChipManager.Instance != null && ChipManager.Instance.HasAllChips())
            {
                if (HackingMinigameManager.Instance != null)
                {
                    HackingMinigameManager.Instance.StartGame(door);
                }
                else
                {
                    Debug.LogError("HackingMinigameManager не найден в сцене!");
                    door.ActivateGlitchWithDelay(0f);
                }
            }
            else
            {
                Debug.Log("Недостаточно чипов!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInZone = false;
        }
    }
}

using UnityEngine;

public class FirstTLevel4 : MonoBehaviour
{
    private FirstCLevel4 c;


    void Start()
    {
        c = FindObjectOfType<FirstCLevel4>();
        if (InventoryManager.instance == null)
            InventoryManager.instance = FindObjectOfType<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Проверяем, что инвентарь существует и в первом слоте есть предмет
            if (InventoryManager.instance != null && InventoryManager.instance.IsFirstSlotOccupied())
            {
                c.StartCoroutine(c.CompleteStartSequence());
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Триггер не активирован: инвентарь пуст или нет первого предмета.");
            }
        }
    }
}

using UnityEngine;

public class QueT : MonoBehaviour
{
    private QueC c;


    void Start()
    {
        c = FindObjectOfType<QueC>();
        if (InventoryManager.instance == null)
            InventoryManager.instance = FindObjectOfType<InventoryManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {



            c.StartCoroutine(c.CompleteStartSequence());
            gameObject.SetActive(false);



        }
    }
}

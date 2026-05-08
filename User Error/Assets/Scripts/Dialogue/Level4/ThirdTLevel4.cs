using UnityEngine;

public class ThirdTLevel4 : MonoBehaviour
{
    private ThirdCLevel4 c;


    void Start()
    {
        c = FindObjectOfType<ThirdCLevel4>();
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

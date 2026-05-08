using UnityEngine;

public class SecondTLevel4 : MonoBehaviour
{
    private SecondCLevel4 c;


    void Start()
    {
        c = FindObjectOfType<SecondCLevel4>();
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

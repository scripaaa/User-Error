using UnityEngine;

public class TriggerActivator : MonoBehaviour
{
    public GameObject[] objectsToActivate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }
}

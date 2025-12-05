using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameObject finishCanvas;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            finishCanvas.SetActive(true);
        }
    }
}

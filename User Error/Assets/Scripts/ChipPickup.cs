using UnityEngine;

public class ChipPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ChipManager.Instance.AddChip();
            Destroy(gameObject);
        }
    }
}

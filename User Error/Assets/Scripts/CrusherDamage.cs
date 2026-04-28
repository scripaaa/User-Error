using UnityEngine;

public class CrusherDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Hero.Instance != null)
                Hero.Instance.Die();
        }
    }
}

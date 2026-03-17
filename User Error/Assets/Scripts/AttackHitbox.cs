using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 0.1f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
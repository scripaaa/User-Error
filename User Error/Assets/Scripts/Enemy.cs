using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.2f;

    private int currentHealth;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Color originalColor;

    private void Awake()
    {
        currentHealth = maxHealth;
        sprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = sprite.color;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        StopAllCoroutines();
        StartCoroutine(FlashEffect());

        ApplyKnockback();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback()
    {
        if (rb == null) return;

        Vector2 direction = (transform.position - Hero.Instance.transform.position).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    private IEnumerator FlashEffect()
    {
        sprite.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        sprite.color = originalColor;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}

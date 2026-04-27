using UnityEngine;

public class BossEnemy : Enemy
{
    protected override void Die()
    {
        // Босс НЕ умирает через Enemy
        // смерть обрабатывается в BossSlime
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Hero.Instance != null)
                Hero.Instance.Die();
        }
    }
}

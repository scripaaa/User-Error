using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 0.1f;

    private List<GameObject> hitEnemies = new List<GameObject>();

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Проверка на обычного врага (ваш старый код)
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && !(enemy is BossEnemy) && !hitEnemies.Contains(collision.gameObject))
        {
            enemy.TakeDamage(damage);
            hitEnemies.Add(collision.gameObject);
            return;
        }

        // 2. Проверка на босса (ваш старый код)
        BossSlime boss = collision.GetComponent<BossSlime>();
        if (boss != null && !hitEnemies.Contains(collision.gameObject))
        {
            boss.TakeDamage(damage);
            hitEnemies.Add(collision.gameObject);
            return; // Добавим return для порядка
        }

        // 3. НОВОЕ: Проверка на стену
        WallDestructor wall = collision.GetComponent<WallDestructor>();
        if (wall != null && !hitEnemies.Contains(collision.gameObject))
        {
            wall.DestroyWall(); // Вызываем ваш метод разрушения
            hitEnemies.Add(collision.gameObject);
        }
    }
}

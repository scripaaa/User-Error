using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 0.1f; // Как долго живет хитбокс

    // Список врагов, которые уже получили урон от ЭТОГО конкретного взмаха
    private List<GameObject> hitEnemies = new List<GameObject>();

    private void Start()
    {
        // Удаляем хитбокс через короткое время, чтобы он не висел в воздухе
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что у объекта есть компонент Enemy и мы его еще не били
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null && !hitEnemies.Contains(collision.gameObject))
        {
            enemy.TakeDamage(damage);
            hitEnemies.Add(collision.gameObject); // Запоминаем врага
        }
    }
}
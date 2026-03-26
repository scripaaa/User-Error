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
       
        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null && !hitEnemies.Contains(collision.gameObject))
        {
            enemy.TakeDamage(damage);
            hitEnemies.Add(collision.gameObject); 
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingMonster : Entity
{
    private float speed = 3.5f;
    private float dir = 1f;
    private SpriteRenderer sprite;


    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        dir = 1f;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 checkPosition = transform.position + Vector3.up * 0.3f + Vector3.right * dir * 0.5f;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPosition, 0.15f);

        bool shouldFlip = false;
        foreach (var collider in colliders)
        {
            if (collider.transform.root == transform.root) continue;
            if (collider.isTrigger) continue;
            if (collider.CompareTag("Player")) continue;
        
            shouldFlip = true;
            break;
        }

        if (shouldFlip) 
        {
            dir *= -1f;
        }

        Vector3 moveDir = Vector3.right * dir;
        transform.position = Vector3.MoveTowards(
            transform.position, 
            transform.position + moveDir, 
            speed * Time.deltaTime
        );

        sprite.flipX = dir > 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.Die();
        }
    }
}
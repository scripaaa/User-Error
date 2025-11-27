using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingMonster : Entity
{
	
    private float speed = 3.5f;
    private Vector3 dir;
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Start is called before the first frame update
    private void Start()
    {
       dir = transform.right; 
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    private void Move()
    {
	    Collider2D[] colliders = Physics2D.OverlapCircleAll(
        transform.position + transform.up * 0.1f + transform.right * dir.x * 0.4f,
        0.1f);
        bool shouldFlip = false;
        foreach (var collider in colliders)
        {
            if (collider.gameObject == gameObject) continue;

            if (collider.isTrigger) continue;

            if (collider.CompareTag("Player")) continue;

            shouldFlip = true;
            break;
        }

        if (shouldFlip) dir *= -1f;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x > 0.0f;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
	    if (collision.gameObject == Hero.Instance.gameObject)
	    {
	        Hero.Instance.Die();
	    }
    }

}

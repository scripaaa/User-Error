using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingMonster : Entity
{
    [Header("Настройки монстра")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float returnSpeed = 1.3f;
    [SerializeField] private float heightThreshold = 1.5f;

    private SpriteRenderer sprite;
    private Vector3 startPosition;
    private Transform player;
    private bool isChasing = false;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        startPosition = transform.position;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                ReturnToStart();
                return;
            }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float heightDifference = player.position.y - transform.position.y;

        if (distanceToPlayer <= detectionRadius && heightDifference < heightThreshold)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            ReturnToStart();
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        transform.position = Vector3.MoveTowards(
            transform.position,
            transform.position + direction,
            speed * Time.deltaTime
        );

        if (direction.x > 0f)
        {
            sprite.flipX = true;
        }
        else if (direction.x < 0f)
        {
            sprite.flipX = false;
        }
    }

    private void ReturnToStart()
    {
        float distanceToStart = Vector3.Distance(transform.position, startPosition);

        if (distanceToStart > 0.1f)
        {
            Vector3 direction = (startPosition - transform.position).normalized;
            direction.y = 0;

            transform.position = Vector3.MoveTowards(
                transform.position,
                startPosition,
                returnSpeed * Time.deltaTime
            );

            if (direction.x > 0f)
            {
                sprite.flipX = true;
            }
            else if (direction.x < 0f)
            {
                sprite.flipX = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.Die();
        }
    }
}
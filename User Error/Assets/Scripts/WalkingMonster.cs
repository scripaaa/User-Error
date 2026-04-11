using UnityEngine;

public class WalkingMonster : Entity
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float chaseSpeed = 4.0f;

    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    private Transform currentTarget;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;

    private SpriteRenderer sprite;
    private bool isChasing = false;
    private Transform playerTransform;

    [Header("Audio Settings")]
    [SerializeField] private float moveSoundInterval = 0.8f;
    [SerializeField] private float moveSoundChance = 0.3f;

    private float moveSoundTimer = 0f;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (pointA != null) currentTarget = pointA;
    }

    private void Update()
    {
        SearchForPlayer();

        bool isMoving = false;

        if (isChasing && playerTransform != null)
        {
            ChasePlayer();
            isMoving = true;
        }
        else
        {
            Patrol();
            isMoving = currentTarget != null && Vector2.Distance(transform.position, new Vector2(currentTarget.position.x, transform.position.y)) > 0.2f;
        }

        HandleSlimeSounds(isMoving);
    }

    private void HandleSlimeSounds(bool isMoving)
    {
        if (!isMoving) return;

        moveSoundTimer -= Time.deltaTime;

        if (moveSoundTimer <= 0f)
        {
            if (Random.value < moveSoundChance)
            {
                if (AudioController.Instance != null)
                {
                    AudioController.Instance.PlaySlimeMovement();
                }
            }

            moveSoundTimer = moveSoundInterval;
        }
    }

    private void SearchForPlayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        bool foundInThisFrame = false;

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player") && col.gameObject != gameObject)
            {
                playerTransform = col.transform;
                foundInThisFrame = true;
                break;
            }
        }

        isChasing = foundInThisFrame;
    }

    private void Patrol()
    {
        if (currentTarget == null) return;

        MoveToTarget(currentTarget.position, speed);

        if (Vector2.Distance(transform.position, new Vector2(currentTarget.position.x, transform.position.y)) < 0.2f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    private void ChasePlayer()
    {
        MoveToTarget(playerTransform.position, chaseSpeed);
    }

    private void MoveToTarget(Vector3 targetPos, float currentSpeed)
    {
        float step = currentSpeed * Time.deltaTime;

        Vector2 newPos = Vector2.MoveTowards(
            transform.position,
            new Vector2(targetPos.x, transform.position.y),
            step
        );

        transform.position = newPos;

        float direction = targetPos.x - transform.position.x;
        if (Mathf.Abs(direction) > 0.01f)
        {
            sprite.flipX = direction < 0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isChasing ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}

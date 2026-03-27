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

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        // Инициализируем начальную точку
        if (pointA != null) currentTarget = pointA;
    }

    private void Update()
    {
        SearchForPlayer();

        if (isChasing && playerTransform != null)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void SearchForPlayer()
    {
        // Находим все объекты в радиусе
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        bool foundInThisFrame = false;

        foreach (var col in colliders)
        {
            // Проверяем ТЕГ и убеждаемся, что это не сам монстр
            if (col.CompareTag("Player") && col.gameObject != gameObject)
            {
                playerTransform = col.transform;
                foundInThisFrame = true;
                break; // Игрок найден, выходим из цикла
            }
        }

        isChasing = foundInThisFrame;
    }

    private void Patrol()
    {
        if (currentTarget == null) return;

        MoveToTarget(currentTarget.position, speed);

        // Дистанция до точки патруля
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

        // Двигаемся только по X
        Vector2 newPos = Vector2.MoveTowards(
            transform.position,
            new Vector2(targetPos.x, transform.position.y),
            step
        );

        transform.position = newPos;

        // Разворот спрайта
        float direction = targetPos.x - transform.position.x;
        if (Mathf.Abs(direction) > 0.01f)
        {
            // Если монстр идет вправо (direction > 0), flipX = false (если спрайт смотрит вправо)
            // Отрегулируй это под свой спрайт
            sprite.flipX = direction < 0f;
        }
    }

    private void OnDrawGizmos()
    {
        // Рисуем радиус агрессии в окне Scene
        Gizmos.color = isChasing ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Рисуем линии к точкам патруля
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
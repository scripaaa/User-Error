using System.Collections.Generic;
using UnityEngine;

public class Hero : Entity
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 11.5f;
    [SerializeField] private float groundCheckRadius = 0.27f;
    [SerializeField] private LayerMask whatIsGround;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("References")]
    [SerializeField] private Transform groundCheck;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    public int countCollectedItems = 0;
    private float wallJumpLockTimer;

    [Header("Juice Settings")]
    [SerializeField] private float coyoteTime = 0.15f; // Длительность окна койота
    private float coyoteTimeCounter;

    public static Hero Instance { get; set; }

    private bool isGrounded = false;
    private bool jumpPerformedThisFrame = false;
    private bool isDashing;
    private bool canDash = true;
    private Vector2 dashDirection;
    private float originalGravity;
    private RoomManager roomManager;

    [Header("Wall jump Settings")]
    [SerializeField] private float wallJumpForce = 11f;
    [SerializeField] private float wallJumpHorizontalForce = 8f;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private float wallSlideSpeed = 2f;

    private bool isTouchingWall = false;
    private int wallDirection = 0;
    private bool canWallJump = true;
    private bool isWallSliding = false;

    [Header("Attack Settings")]
    [SerializeField] private GameObject attackHitboxPrefab;
    [SerializeField] private Transform attackPoint;

    [Header("Audio Settings")]
    [SerializeField] private float footstepInterval = 0.35f;
    [SerializeField] private float minMovementForFootstep = 0.1f;

    private float footstepTimer = 0f;
    private bool wasMoving = false;

    private MovingPlatform currentPlatform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        originalGravity = rb.gravityScale;
        anim = GetComponent<Animator>();

        if (Instance == null)
        {
            Instance = this;
        }

        // Делаем герою статический доступ, если понадобится из других скриптов
        Instance = this;

        // Сохраняем ссылку на менеджер уже здесь, а не в Start (чтобы он точно был готов)
        roomManager = FindObjectOfType<RoomManager>();
        if (roomManager == null)
            Debug.LogError("[Hero] RoomManager не найден в сцене!");
    }

    private void FixedUpdate()
    {
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive())
            return;

        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            return;
        }

        CheckWall();
        HandleWallSliding();
        Jump();
    }

    private void Update()
    {
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        CheckGround();

        if (!jumpPerformedThisFrame)
            anim.SetBool("grounded", isGrounded);

        float moveInput = Input.GetAxisRaw("Horizontal"); // Используем GetAxisRaw для резкости
        anim.SetBool("Run", Mathf.Abs(moveInput) > 0.1f);

        if (isDashing) return;

        if (wallJumpLockTimer <= 0)
        {
            Run(moveInput);
        }
        else
        {
            wallJumpLockTimer -= Time.deltaTime;
        }
        // Прыжок и Даш
        if (Input.GetButtonDown("Jump")) jumpPerformedThisFrame = true;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q))
        {
            Attack();
        }

        HandleFootstepSounds(moveInput);
    }

    private void HandleFootstepSounds(float moveInput)
    {
        bool isMoving = Mathf.Abs(moveInput) > minMovementForFootstep && isGrounded && !isDashing;

        if (isMoving)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                if (AudioController.Instance != null)
                {
                    AudioController.Instance.PlayFootstepSound();
                }

                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        wasMoving = isMoving;
    }

    private void Run(float moveInput)
    {
        float platformX = currentPlatform != null ? currentPlatform.PlatformVelocity.x : 0f;

        rb.linearVelocity = new Vector2(moveInput * speed + platformX, rb.linearVelocity.y);

        bool isUpsideDown = Mathf.Abs(transform.eulerAngles.z - 180f) < 0.1f;
        float correctedInput = isUpsideDown ? -moveInput : moveInput;

        if (correctedInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (correctedInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void Jump()
    {
        if (isDashing) return;
        CheckGround();

        // Определяем направление гравитации: -1 (вниз), 1 (вверх)
        float gravityDir = Mathf.Sign(Physics2D.gravity.y);

        // 1. Обычный прыжок
        if (jumpPerformedThisFrame && coyoteTimeCounter > 0f)
        {
            anim.SetTrigger("Jump");

            // Прыгаем ПРОТИВ направления гравитации
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * -gravityDir);

            coyoteTimeCounter = 0f;
            jumpPerformedThisFrame = false;
            return;
        }

        // 2. Прыжок от стены
        if (jumpPerformedThisFrame && !isGrounded && isTouchingWall && canWallJump)
        {
            canWallJump = false;
            float hor = (wallDirection == -1) ? 1f : -1f;

            // Также инвертируем вертикальную силу прыжка от стены
            rb.linearVelocity = new Vector2(hor * wallJumpHorizontalForce, wallJumpForce * -gravityDir);

            wallJumpLockTimer = 0.15f;
            transform.localScale = new Vector3(hor, transform.localScale.y, 1);

            jumpPerformedThisFrame = false;
            return;
        }

        jumpPerformedThisFrame = false;
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, whatIsGround);
        bool wasGrounded = isGrounded;
        isGrounded = false;

        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != gameObject)
            {
                isGrounded = true;
                canWallJump = true;
                break;
            }
        }

        // Если мы на земле — счетчик на максимуме. 
        // Если в воздухе — он начинает уменьшаться.
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }
    /// <summary>
    /// CheackWall
    /// </summary>
    private void CheckWall()
    {
        if (isGrounded)
        {
            isTouchingWall = false;
            return;
        }
        isTouchingWall = false;
        wallDirection = 0;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        Bounds bounds = col.bounds;
        // Небольшой отступ внутрь, чтобы лучи не начинались на самой границе
        float inset = 0.05f;

        // Рисуем 3 луча с каждой стороны
        int rayCount = 3;
        float verticalStep = (bounds.max.y - bounds.min.y) / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float yPos = bounds.min.y + (verticalStep * i);

            // Точки старта чуть-чуть внутри коллайдера
            Vector2 leftStart = new Vector2(bounds.min.x + inset, yPos);
            Vector2 rightStart = new Vector2(bounds.max.x - inset, yPos);

            // Дистанция должна учитывать наш inset + небольшой запас
            float distance = inset + 0.1f;

            RaycastHit2D hitLeft = Physics2D.Raycast(leftStart, Vector2.left, distance, whatIsWall);
            RaycastHit2D hitRight = Physics2D.Raycast(rightStart, Vector2.right, distance, whatIsWall);

            // Визуализация в эдиторе
            Debug.DrawRay(leftStart, Vector2.left * distance, hitLeft ? Color.green : Color.red);
            Debug.DrawRay(rightStart, Vector2.right * distance, hitRight ? Color.green : Color.blue);

            if (hitLeft.collider != null)
            {
                isTouchingWall = true;
                wallDirection = -1;
                break;
            }
            if (hitRight.collider != null)
            {
                isTouchingWall = true;
                wallDirection = 1;
                break;
            }
        }

        if (!isTouchingWall && !isGrounded)
            canWallJump = true;
    }
    private void HandleWallSliding()
    {
        float gravityDir = Mathf.Sign(Physics2D.gravity.y);

        // Проверяем "падение": 
        // Если гравитация обычная (-1), падаем когда velocity.y < 0
        // Если инвертированная (1), падаем когда velocity.y > 0
        bool isFalling = (gravityDir < 0) ? rb.linearVelocity.y < 0 : rb.linearVelocity.y > 0;

        if (isTouchingWall && !isGrounded && isFalling)
        {
            isWallSliding = true;
            // Скользим "вниз" относительно текущей гравитации
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideSpeed * gravityDir);
        }
        else
        {
            isWallSliding = false;
        }
    }

    /// <summary>
    /// ������ �����
    /// </summary>
    private void StartDash()
    {
        isDashing = true;
        canDash = false;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        dashDirection = new Vector2(horizontalInput, 0f).normalized;
        rb.gravityScale = 0;
        Invoke(nameof(EndDash), dashDuration);
        Invoke(nameof(ResetDashCooldown), dashCooldown);
    }

    private void ResetDashCooldown()
    {
        canDash = true;
    }

    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravity;
    }

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
    }

    public void Die()
    {
        if (DeathScreenManager.Instance != null && !DeathScreenManager.Instance.IsFading)
        {
            DeathScreenManager.Instance.ShowDeathScreen(() =>
            {
                PerformRespawn();
            });
            return;
        }

        PerformRespawn();
    }

    private void PerformRespawn()
    {
        if (LevelCheckpointManager.Instance != null)
        {
            LevelCheckpointManager.Instance.RespawnHero();
            return;
        }

        if (roomManager != null)
        {
            roomManager.Respawn(gameObject);
            return;
        }

        Debug.LogWarning("[Hero] Нет ни LevelCheckpointManager, ни RoomManager – персонаж не будет перемещён после смерти.");

        footstepTimer = 0f;
        wasMoving = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    private void Attack()
    {
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive()) return;
        if (isDashing) return;

        anim.SetTrigger("Attack");

        Instantiate(attackHitboxPrefab, attackPoint.position, transform.rotation);
    }

    // платформа
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var platform = collision.gameObject.GetComponent<MovingPlatform>();
        if (platform != null) currentPlatform = platform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<MovingPlatform>() != null)
            currentPlatform = null;
    }
}
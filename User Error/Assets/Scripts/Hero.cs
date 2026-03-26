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

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

    }

    private void Run(float moveInput)
    {
        // Устанавливаем горизонтальную скорость, сохраняя вертикальную (гравитацию)
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // Поворот через localScale
        if (moveInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void Jump()
    {
        if (isDashing) return;
        CheckGround();

       

        // Прыжок с земли (теперь с учетом Coyote Time)
        if (jumpPerformedThisFrame && coyoteTimeCounter > 0f)
        {
            anim.SetTrigger("Jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // Важно: обнуляем счетчик после прыжка, чтобы нельзя было 
            // прыгнуть второй раз в воздухе
            coyoteTimeCounter = 0f;
            jumpPerformedThisFrame = false;
            return;
        }

        if (jumpPerformedThisFrame && !isGrounded && isTouchingWall && canWallJump)
        {
            canWallJump = false;
            float hor = (wallDirection == -1) ? 1f : -1f;

            // Прикладываем силу
            rb.linearVelocity = new Vector2(hor * wallJumpHorizontalForce, wallJumpForce);

            // Блокируем ввод на 0.15 секунды, чтобы персонаж успел отлететь от стены
            wallJumpLockTimer = 0.15f;

            // Разворачиваем персонажа в сторону прыжка
            transform.localScale = new Vector3(hor, 1, 1);

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
        // Скольжение возможно, если:
        // - касаемся стены
        // - не на земле
        // - падаем вниз (velocity.y < 0)
        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            // Устанавливаем вертикальную скорость равной wallSlideSpeed (отрицательной)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
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
        roomManager.Respawn(gameObject);
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
}
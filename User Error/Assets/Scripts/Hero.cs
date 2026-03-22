using System.Collections.Generic;
using UnityEngine;


public class Hero : Entity
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f; // �������� ��������
    [SerializeField] private float jumpForce = 11f; // ���� ������
    [SerializeField] private float groundCheckRadius = 0.27f; //������ �������� ����� 
    [SerializeField] private LayerMask whatIsGround; // ����� ����, ������������ ��� ��������� ����� 
    public float dashSpeed = 15f; // �� ��� ������� � ������ 
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public KeyCode dashKey = KeyCode.LeftShift; // ����� ������� �� shift


    [Header("References")]
    [SerializeField] private Transform groundCheck;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    public static Hero Instance { get; set; }

    private bool isGrounded = false;
    private bool jumpPerformedThisFrame = false;
    private bool isDashing; // �� ��� ������� � ������ 
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
            return; // ��������� ���������� ��������� ������
        }
       
        CheckWall();
        HandleWallSliding();
        Jump();


    }
    private void Update()
    {
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive())
            return;
        CheckGround();
        if (!jumpPerformedThisFrame)
         anim.SetBool("grounded", isGrounded);

        if (isDashing) return;

        if (Input.GetButton("Horizontal"))
            Run();
        anim.SetBool("Run", Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));
        
        if (Input.GetButtonDown("Jump"))
        {
            jumpPerformedThisFrame = true;

        }
        
        if (Input.GetKeyDown(dashKey) && canDash && Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
        {
            StartDash();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

    }

    private void Run()
    {
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        sprite.flipX = dir.x < 0.0f;
    }
    private void Jump()
    {
        if (isDashing) return;
        CheckGround();

        if (jumpPerformedThisFrame)
            anim.SetBool("grounded", false);

        if (jumpPerformedThisFrame && isGrounded)
        {
            anim.SetTrigger("Jump");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPerformedThisFrame = false;
            return;
        }


        if(jumpPerformedThisFrame && !isGrounded && isTouchingWall && canWallJump)
        {
            canWallJump = false;
            float hor = (wallDirection == -1) ? 1f : -1f;
            rb.linearVelocity = new Vector2(hor * wallJumpHorizontalForce,wallJumpForce);

            jumpPerformedThisFrame = false;
            return;
        }

        jumpPerformedThisFrame = false;

    }
    /// <summary>
    /// ������� �������� ����� 
    /// </summary>
    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, whatIsGround);


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
    }
    /// <summary>
    /// CheackWall
    /// </summary>
    private void CheckWall()
    {
        isTouchingWall = false;
        wallDirection = 0;

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        // Получаем границы коллайдера
        Bounds bounds = col.bounds;

        // Определяем количество лучей и их смещения по Y (относительно нижней и верхней границы)
        int rayCount = 3; // можно увеличить при необходимости
        float[] offsets = new float[rayCount];

        if (rayCount == 1)
        {
            offsets[0] = bounds.center.y;
        }
        else
        {
            for (int i = 0; i < rayCount; i++)
            {
                // Равномерно распределяем лучи по высоте коллайдера
                float t = i / (float)(rayCount - 1); // от 0 до 1
                offsets[i] = Mathf.Lerp(bounds.min.y, bounds.max.y, t);
            }
        }

        // Переменные для отслеживания попаданий
        bool leftHit = false;
        bool rightHit = false;

        // Проходим по всем точкам и пускаем лучи
        foreach (float y in offsets)
        {
            Vector2 leftOrigin = new Vector2(bounds.min.x, y);
            Vector2 rightOrigin = new Vector2(bounds.max.x, y);

            RaycastHit2D hitLeft = Physics2D.Raycast(leftOrigin, Vector2.left, wallCheckDistance, whatIsWall);
            RaycastHit2D hitRight = Physics2D.Raycast(rightOrigin, Vector2.right, wallCheckDistance, whatIsWall);

            // Рисуем лучи для отладки (каждый луч своим цветом: красный для левого, синий для правого)
            Debug.DrawRay(leftOrigin, Vector2.left * wallCheckDistance, hitLeft.collider != null ? Color.green : Color.red);
            Debug.DrawRay(rightOrigin, Vector2.right * wallCheckDistance, hitRight.collider != null ? Color.green : Color.blue);

            if (hitLeft.collider != null)
                leftHit = true;
            if (hitRight.collider != null)
                rightHit = true;
        }

        // Определяем результат
        if (leftHit)
        {
            isTouchingWall = true;
            wallDirection = -1;
        }
        else if (rightHit)
        {
            isTouchingWall = true;
            wallDirection = 1;
        }

        // Разрешаем стена-прыжок, если персонаж не касается стены и не на земле
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

        // ���������� ����������� ���� �� ������ ���������� �����
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        dashDirection = new Vector2(horizontalInput, 0f).normalized;

        // ��������� ���������� �� ����� ����
        rb.gravityScale = 0;

        // ��������� �������
        Invoke(nameof(EndDash), dashDuration);
        Invoke(nameof(ResetDashCooldown), dashCooldown);
    }


    /// <summary>
    /// ����� �������� �����
    /// </summary>
    private void ResetDashCooldown()
    {
        canDash = true;
    }
    /// <summary>
    /// ���������� �����
    /// </summary>
    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravity; // ��������������� ����������
    }

    private void Start()
    {
        roomManager = FindObjectOfType<RoomManager>();
    }

    // ������
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
        Vector3 spawnPos = attackPoint.position;

        if (sprite.flipX)
            spawnPos.x = transform.position.x - Mathf.Abs(attackPoint.localPosition.x);
        else
            spawnPos.x = transform.position.x + Mathf.Abs(attackPoint.localPosition.x);

        Instantiate(attackHitboxPrefab, spawnPos, Quaternion.identity);
    }
}
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

    private bool isTouchingWall = false;
    private int wallDirection = 0;
    private bool canWallJump = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        originalGravity = rb.gravityScale;

        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {

            rb.linearVelocity = dashDirection * dashSpeed;
            return; // ��������� ���������� ��������� ������
        }
        CheckGround();
        CheckWall();
        Jump();
    }
    private void Update()
    {
        if (isDashing) return;

        if (Input.GetButton("Horizontal"))
            Run();
        if (Input.GetButtonDown("Jump"))
        {
            jumpPerformedThisFrame = true;

        }
        if (Input.GetKeyDown(dashKey) && canDash && Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
        {
            StartDash();
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


        if (jumpPerformedThisFrame && isGrounded)
        {

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
                break;
            }
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

}
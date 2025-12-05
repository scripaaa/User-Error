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

    private bool isTouchingWall = false;
    private int wallDirection = 0;
    private bool canWallJump = true;

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

        Vector2 leftOrigin = new Vector2(col.bounds.min.x, transform.position.y);
        Vector2 rightOrigin = new Vector2(col.bounds.max.x, transform.position.y);

        RaycastHit2D hitLeft = Physics2D.Raycast(leftOrigin, Vector2.left, wallCheckDistance, whatIsWall);
        RaycastHit2D hitRight = Physics2D.Raycast(rightOrigin, Vector2.right, wallCheckDistance, whatIsWall);

        Debug.DrawRay(leftOrigin, Vector2.left * wallCheckDistance, Color.red);
        Debug.DrawRay(rightOrigin, Vector2.right * wallCheckDistance, Color.blue);

        if (hitLeft.collider != null)
        {
            isTouchingWall = true;
            wallDirection = -1;
        }
        else if (hitRight.collider != null)
        {
            isTouchingWall = true;
            wallDirection = 1;

        }
        if (!isTouchingWall && !isGrounded)
            canWallJump = true;
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
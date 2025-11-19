using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f; // скорость движения
    [SerializeField] private float jumpForce = 11f; // сила прыжка
    [SerializeField] private float groundCheckRadius = 0.27f; //радиус проверки земли 
    [SerializeField] private LayerMask whatIsGround; // Маска слоёв, определяющая что считается землёй 
    public float dashSpeed = 15f; // Всё что связано с рывком 
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public KeyCode dashKey = KeyCode.LeftShift; // рывок нажатие на shift


    [Header("References")]
    [SerializeField] private Transform groundCheck;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private bool isGrounded = false;
    private bool jumpPerformedThisFrame = false;
    private bool isDashing; // Всё что связано с рывком 
    private bool canDash = true;
    private Vector2 dashDirection;
    private float originalGravity;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        originalGravity = rb.gravityScale;
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            
            rb.velocity = dashDirection * dashSpeed;
            return; // Прерываем выполнение остальной логики
        }
        CheckGround();

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
            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        
       
        jumpPerformedThisFrame = false;

    }
    /// <summary>
    /// Система проверки земли 
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
    /// Начало рывка
    /// </summary>
    private void StartDash()
    {
        isDashing = true;
        canDash = false;

        // Определяем направление даша на основе последнего ввода
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        dashDirection = new Vector2(horizontalInput, 0f).normalized;

        // Отключаем гравитацию на время даша
        rb.gravityScale = 0;

        // Запускаем таймеры
        Invoke(nameof(EndDash), dashDuration);
        Invoke(nameof(ResetDashCooldown), dashCooldown);
    }


    /// <summary>
    /// Сброс кулдауна рывка
    /// </summary>
    private void ResetDashCooldown()
    {
        canDash = true;
    }
    /// <summary>
    /// Завершение рывка
    /// </summary>
    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = originalGravity; // Восстанавливаем гравитацию
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

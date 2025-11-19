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
    

    [Header("References")]
    [SerializeField] private Transform groundCheck;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;

    private bool isGrounded = false;
    private bool jumpPerformedThisFrame = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        CheckGround();

        Jump();
    }
    private void Update()
    {
        if (Input.GetButton("Horizontal"))
            Run();
        if (Input.GetButtonDown("Jump"))
        {
            jumpPerformedThisFrame = true;
            
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
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

}

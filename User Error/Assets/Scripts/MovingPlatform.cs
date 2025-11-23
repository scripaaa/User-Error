using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float travelTime = 2f;
    public float pauseTime = 1f;

    private Rigidbody2D rb;
    private float t = 0f;
    private bool movingAB = true;
    private bool isPaused = false;
    private Vector2 previousPosition;

    private readonly List<Rigidbody2D> carriedBodies = new List<Rigidbody2D>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Start()
    {
        previousPosition = rb.position;
    }

    void FixedUpdate()
    {
        if (isPaused) return;

        t += Time.fixedDeltaTime / travelTime;
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        Vector2 targetPos = movingAB ?
            Vector2.Lerp(pointA.position, pointB.position, smoothT) :
            Vector2.Lerp(pointB.position, pointA.position, smoothT);

        rb.MovePosition(targetPos);

        Vector2 delta = targetPos - previousPosition;

        if (delta != Vector2.zero)
        {
            foreach (var body in carriedBodies.ToArray())
            {
                if (body != null)
                    body.MovePosition(body.position + delta);
            }
        }

        previousPosition = targetPos;

        if (t >= 1f)
            StartCoroutine(PauseAndReverse());
    }

    private IEnumerator PauseAndReverse()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseTime);
        movingAB = !movingAB;
        t = 0f;
        isPaused = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D otherRb = collision.collider.attachedRigidbody;
        if (otherRb != null && !carriedBodies.Contains(otherRb))
        {
            foreach (var c in collision.contacts)
            {
                if (c.normal.y > 0.5f) // объект сверху
                {
                    carriedBodies.Add(otherRb);
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D otherRb = collision.collider.attachedRigidbody;
        if (otherRb != null)
        {
            carriedBodies.Remove(otherRb);
        }
    }
}

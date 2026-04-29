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

    public Vector2 PlatformVelocity { get; private set; }

    private Rigidbody2D rb;
    private float t = 0f;
    private bool movingAB = true;
    private bool isPaused = false;
    private Vector2 previousPosition;

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
        if (isPaused)
        {
            PlatformVelocity = Vector2.zero;
            return;
        }

        t += Time.fixedDeltaTime / travelTime;
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        Vector2 targetPos = movingAB
            ? Vector2.Lerp(pointA.position, pointB.position, smoothT)
            : Vector2.Lerp(pointB.position, pointA.position, smoothT);

        rb.MovePosition(targetPos);

        Vector2 delta = targetPos - previousPosition;
        PlatformVelocity = delta / Time.fixedDeltaTime;

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
}
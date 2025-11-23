using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float travelTime = 2f;
    public float pauseTime = 1f;

    private bool movingAB = true;
    private bool isPaused = false;
    private float t = 0f;

    void Update()
    {
        if (isPaused) return;

        t += Time.deltaTime / travelTime;
        float smoothT = Mathf.SmoothStep(0, 1, t);

        if (movingAB)
            transform.position = Vector3.Lerp(pointA.position, pointB.position, smoothT);
        else
            transform.position = Vector3.Lerp(pointB.position, pointA.position, smoothT);

        if (t >= 1f)
        {
            StartCoroutine(PauseAndReverse());
        }
    }

    private System.Collections.IEnumerator PauseAndReverse()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseTime);

        movingAB = !movingAB;
        t = 0f;
        isPaused = false;
    }
}

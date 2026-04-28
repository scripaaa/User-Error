using UnityEngine;

public class Crusher : MonoBehaviour
{
    public Transform block;          // CrusherBlock
    public float upDistance = 3f;    // Насколько высоко поднимается
    public float speed = 5f;         // Скорость движения
    public float waitTime = 0.5f;    // Пауза перед падением

    private Vector3 startPos;
    private Vector3 upPos;
    private bool goingUp = true;
    private float timer = 0f;

    void Start()
    {
        startPos = block.position;
        upPos = startPos + Vector3.up * upDistance;
    }

    void Update()
    {
        if (goingUp)
        {
            block.position = Vector3.MoveTowards(block.position, upPos, speed * Time.deltaTime);

            if (Vector3.Distance(block.position, upPos) < 0.01f)
            {
                timer += Time.deltaTime;
                if (timer >= waitTime)
                {
                    goingUp = false;
                    timer = 0f;
                }
            }
        }
        else
        {
            block.position = Vector3.MoveTowards(block.position, startPos, speed * 2 * Time.deltaTime);

            if (Vector3.Distance(block.position, startPos) < 0.01f)
            {
                timer += Time.deltaTime;
                if (timer >= waitTime)
                {
                    goingUp = true;
                    timer = 0f;
                }
            }
        }
    }
}

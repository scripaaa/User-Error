using UnityEngine;

public class FlailSwing : MonoBehaviour
{
    public float speed = 100f;

    void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
    }
}

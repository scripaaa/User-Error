using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public Vector2 gravityVector = new Vector2(0f, 9.81f);
    public bool resetOnDisable = true;
    private Vector2 originalGravity;
    void OnEnable()
    {
        originalGravity = Physics2D.gravity;
        Physics2D.gravity = gravityVector;
    }

    void OnDisable()
    {
        if (resetOnDisable)
        {
            Physics2D.gravity = originalGravity;
        }
    }

   
}

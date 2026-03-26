using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public Vector2 gravityVector = new Vector2(0f, 9.81f); // Положительный Y — это гравитация вверх
    public bool resetOnDisable = true;

    private Vector2 originalGravity;

    void OnEnable()
    {
        originalGravity = Physics2D.gravity;
        Physics2D.gravity = gravityVector;

        // Поворачиваем героя физически, чтобы GroundCheck (который дочерний объект)
        // оказался сверху и касался потолка.
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.eulerAngles = new Vector3(0, 0, gravityVector.y > 0 ? 180f : 0f);
        }
    }

    void FlipPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (gravityVector.y > 0)
            {
                player.transform.rotation = Quaternion.Euler(0, 0, 180f);
            }
            else
            {
                player.transform.rotation = Quaternion.identity;
            }
        }
    }

    void OnDisable()
    {
        if (resetOnDisable)
        {
            Physics2D.gravity = originalGravity;
        }
    }
}

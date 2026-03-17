using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteVisibilityController : MonoBehaviour
{
    public Transform player;
    public SpriteRenderer spriteRenderer;
     float maxDistance = 10f; // Максимальное расстояние, при котором текст будет виден
     float minAlpha = 0f; // Минимальный уровень прозрачности текста
     float maxAlpha = 1f;  // Максимальный уровень прозрачности текста
     float fullDistance = 5f;




    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = Hero.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        float alpha;
        if (distance <= fullDistance)
        {
            alpha = maxAlpha; // Полная видимость
        }
        else if (distance >= maxDistance)
        {
            alpha = minAlpha; // Полная прозрачность
        }
        else
        {
            // Плавное изменение между fullDistance и maxDistance
            float t = (distance - fullDistance) / (maxDistance - fullDistance);
            alpha = Mathf.Lerp(maxAlpha, minAlpha, t);
        }

        Color spriteColor = spriteRenderer.color;
        spriteColor.a = alpha;
        spriteRenderer.color = spriteColor;


    }
}

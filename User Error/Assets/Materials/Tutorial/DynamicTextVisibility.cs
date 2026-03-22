using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DynamicTextVisibility : MonoBehaviour
{
    private TextMeshPro textMeshPro;

    public Transform player;
    public float maxDistance = 10f; // ћаксимальное рассто€ние, при котором текст будет виден
    public float minAlpha = 0f; // ћинимальный уровень прозрачности текста
    public float maxAlpha = 3f; // ћаксимальный уровень прозрачности текста

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        player = Hero.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position); // ¬ычисл€ем рассто€ние между игроком и текстом
        float alpha = Mathf.Clamp01(1 - (distance / maxDistance));// Ќормализуем рассто€ние на основе maxDistance
        textMeshPro.alpha = Mathf.Lerp(minAlpha, maxAlpha, alpha); // ”станавливаем уровень альфа по формуле дл€ нормализации

        
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GraphNode : MonoBehaviour
{
    public enum NodeType { Normal, Button, Goal }
    public NodeType type = NodeType.Normal;

    public List<GraphNode> neighbors = new List<GraphNode>();
    public List<GraphNode> lockedNeighbors = new List<GraphNode>();

    [Header("Visuals")]
    public Color normalColor = new Color(0.1f, 0.2f, 0.4f, 1f); // Темно-синий
    public Color activeColor = new Color(0f, 1f, 1f, 1f);       // Циан
    public Color goalColor = new Color(0f, 1f, 0.5f, 1f);      // Неоновый зеленый
    public Color buttonColor = new Color(1f, 0.8f, 0f, 1f);    // Золотой
    public Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Серый

    private Image image;
    private RectTransform rectTransform;
    private Outline outline;

    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        
        // Добавляем обводку для красоты, если её нет
        outline = GetComponent<Outline>();
        if (outline == null) outline = gameObject.AddComponent<Outline>();
        outline.effectDistance = new Vector2(2, -2);
        outline.effectColor = new Color(1, 1, 1, 0.2f);
    }

    public void SetState(bool isCurrent, bool isLocked)
    {
        if (image == null) image = GetComponent<Image>();
        
        if (isLocked)
        {
            image.color = lockedColor;
            return;
        }

        if (isCurrent)
        {
            image.color = activeColor;
            image.rectTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }
        else
        {
            image.rectTransform.localScale = Vector3.one;
            switch (type)
            {
                case NodeType.Goal: image.color = goalColor; break;
                case NodeType.Button: image.color = buttonColor; break;
                default: image.color = normalColor; break;
            }
        }
    }

    public Vector2 GetPosition()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        return rectTransform.anchoredPosition;
    }
}

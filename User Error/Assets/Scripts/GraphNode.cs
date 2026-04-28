using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GraphNode : MonoBehaviour
{
    public enum NodeType { Normal, Button, Goal }
    public NodeType type = NodeType.Normal;

    public List<GraphNode> neighbors = new List<GraphNode>();
    
    [Header("Switch Logic")]
    public List<GraphNode> nodesToToggle = new List<GraphNode>(); // Nodes to connect/disconnect with THIS node
    public List<ConnectionPair> pairsToToggle = new List<ConnectionPair>(); // Custom pairs to connect/disconnect
    public bool isReusable = true;
    private bool wasUsed = false;

    [System.Serializable]
    public struct ConnectionPair {
        public GraphNode a;
        public GraphNode b;
    }

    [Header("Visuals")]
    public Color normalColor = new Color(0.1f, 0.2f, 0.4f, 1f);
    public Color activeColor = new Color(0f, 1f, 1f, 1f);
    public Color goalColor = new Color(0f, 1f, 0.5f, 1f);
    public Color buttonColor = new Color(1f, 0.8f, 0f, 1f);

    private Image image;
    private RectTransform rectTransform;

    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetState(bool isCurrent)
    {
        if (image == null) image = GetComponent<Image>();
        
        if (isCurrent)
        {
            image.color = activeColor;
            transform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }
        else
        {
            transform.localScale = Vector3.one;
            switch (type)
            {
                case NodeType.Goal: image.color = goalColor; break;
                case NodeType.Button: image.color = buttonColor; break;
                default: image.color = normalColor; break;
            }
        }
    }

    public void OnClick()
    {
        if (type != NodeType.Button) return;
        if (wasUsed && !isReusable) return;

        // Toggle connections with self
        foreach (var node in nodesToToggle)
        {
            ToggleConnection(this, node);
        }

        // Toggle custom pairs
        foreach (var pair in pairsToToggle)
        {
            ToggleConnection(pair.a, pair.b);
        }

        wasUsed = isReusable ? !wasUsed : true;
    }

    private void ToggleConnection(GraphNode a, GraphNode b)
    {
        if (a == null || b == null) return;
        if (a.neighbors.Contains(b))
        {
            a.neighbors.Remove(b);
            b.neighbors.Remove(a);
        }
        else
        {
            a.neighbors.Add(b);
            b.neighbors.Add(a);
        }
    }

    public Vector2 GetPosition()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        return rectTransform.anchoredPosition;
    }
}

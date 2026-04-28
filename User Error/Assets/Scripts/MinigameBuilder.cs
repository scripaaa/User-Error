using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MinigameBuilder : MonoBehaviour
{
    public void BuildMinigame()
    {
        // 1. Создаем Canvas
        GameObject canvasObj = new GameObject("HackingCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Создаем Фон (Dark Overlay)
        GameObject panelObj = new GameObject("Background");
        panelObj.transform.SetParent(canvasObj.transform);
        Image bgImage = panelObj.AddComponent<Image>();
        bgImage.color = new Color(0.01f, 0.01f, 0.03f, 0.98f);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        // 3. Маска для "Камеры" (чтобы узлы не вылезали за пределы при скролле, если нужно)
        // Но для простоты оставим просто контейнер.

        // 4. Контейнер для узлов (наша "карта")
        GameObject containerObj = new GameObject("NodeContainer");
        containerObj.transform.SetParent(panelObj.transform);
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(2000, 2000); // Большая карта
        containerRect.anchoredPosition = Vector2.zero;

        // 5. Линия (Prefab)
        GameObject linePrefab = new GameObject("LinePrefab");
        Image lineImg = linePrefab.AddComponent<Image>();
        lineImg.color = new Color(0, 0.8f, 1f, 0.6f);
        RectTransform lineRect = linePrefab.GetComponent<RectTransform>();
        lineRect.pivot = new Vector2(0, 0.5f);
        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        linePrefab.transform.SetParent(canvasObj.transform);
        linePrefab.SetActive(false);

        // 6. Маркер Игрока
        GameObject playerMarker = new GameObject("PlayerMarker");
        playerMarker.transform.SetParent(containerObj.transform);
        Image playerImg = playerMarker.AddComponent<Image>();
        playerImg.color = new Color(0f, 1f, 1f, 1f);
        RectTransform markerRect = playerMarker.GetComponent<RectTransform>();
        markerRect.sizeDelta = new Vector2(45, 45);
        markerRect.anchorMin = new Vector2(0.5f, 0.5f);
        markerRect.anchorMax = new Vector2(0.5f, 0.5f);

        // 7. Построение длинного уровня
        List<GraphNode> nodes = new List<GraphNode>();
        
        // Начало
        GraphNode nStart = CreateNode(containerObj, "Start", new Vector2(-400, 0), GraphNode.NodeType.Normal);
        GraphNode n1 = CreateNode(containerObj, "N1", new Vector2(-200, 100), GraphNode.NodeType.Normal);
        GraphNode n2 = CreateNode(containerObj, "N2", new Vector2(-200, -100), GraphNode.NodeType.Normal);
        
        // Первый замок
        GraphNode nBtn1 = CreateNode(containerObj, "Key1", new Vector2(0, 200), GraphNode.NodeType.Button);
        GraphNode nGate1 = CreateNode(containerObj, "Gate1", new Vector2(0, 0), GraphNode.NodeType.Normal);
        
        // Дальше (скролл вправо)
        GraphNode n3 = CreateNode(containerObj, "N3", new Vector2(300, 100), GraphNode.NodeType.Normal);
        GraphNode n4 = CreateNode(containerObj, "N4", new Vector2(300, -100), GraphNode.NodeType.Normal);
        
        // Второй замок (глубоко справа)
        GraphNode nBtn2 = CreateNode(containerObj, "Key2", new Vector2(500, -200), GraphNode.NodeType.Button);
        GraphNode nGate2 = CreateNode(containerObj, "Gate2", new Vector2(600, 0), GraphNode.NodeType.Normal);
        
        // Финал
        GraphNode nGoal = CreateNode(containerObj, "CORE", new Vector2(900, 0), GraphNode.NodeType.Goal);

        // Связи
        Link(nStart, n1); Link(nStart, n2);
        Link(n1, nBtn1); Link(n2, nGate1);
        Link(nBtn1, nGate1); 
        
        // n3 заблокирован кнопкой 1
        nBtn1.lockedNeighbors.Add(n3);
        Link(nGate1, n4);
        Link(n3, n4);
        Link(n4, nBtn2);
        
        // nGate2 заблокирован кнопкой 2
        nBtn2.lockedNeighbors.Add(nGate2);
        Link(nGate2, nGoal);

        nodes.AddRange(new[] { nStart, n1, n2, nBtn1, nGate1, n3, n4, nBtn2, nGate2, nGoal });

        // 8. Менеджер
        HackingMinigameManager manager = canvasObj.AddComponent<HackingMinigameManager>();
        SetPrivateField(manager, "minigameUI", panelObj);
        SetPrivateField(manager, "nodeContainer", containerRect);
        SetPrivateField(manager, "playerMarker", markerRect);
        SetPrivateField(manager, "linePrefab", linePrefab);
        manager.allNodes = nodes;

        Debug.Log("Мини-игра пересобрана: Длинный уровень + Система камеры готова!");
    }

    private void Link(GraphNode a, GraphNode b) {
        if (!a.neighbors.Contains(b)) a.neighbors.Add(b);
        if (!b.neighbors.Contains(a)) b.neighbors.Add(a);
    }

    private GraphNode CreateNode(GameObject parent, string name, Vector2 pos, GraphNode.NodeType type) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform);
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.2f, 0.4f, 1f);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(50, 50);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        go.AddComponent<Outline>().effectDistance = new Vector2(2, -2);
        GraphNode node = go.AddComponent<GraphNode>();
        node.type = type;
        return node;
    }

    private void SetPrivateField(object obj, string fieldName, object value) {
        System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MinigameBuilder : MonoBehaviour
{
    private const float GRID = 150f;

    public void BuildMinigame()
    {
        GameObject old = GameObject.Find("HackingCanvas");
        if (old) DestroyImmediate(old);

        GameObject canvasObj = new GameObject("HackingCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        HackingMinigameManager manager = canvasObj.AddComponent<HackingMinigameManager>();

        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.01f, 0.01f, 0.03f, 0.99f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero; bgRect.anchoredPosition = Vector2.zero;
        bgObj.SetActive(false);

        GameObject containerObj = new GameObject("NodeContainer");
        containerObj.transform.SetParent(bgObj.transform);
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(10000, 10000);
        containerRect.anchoredPosition = Vector2.zero;

        GameObject linePrefab = new GameObject("LinePrefab");
        linePrefab.AddComponent<Image>().color = new Color(0, 0.7f, 1f, 0.6f);
        RectTransform lineRect = linePrefab.GetComponent<RectTransform>();
        lineRect.pivot = new Vector2(0, 0.5f);
        lineRect.anchorMin = lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        linePrefab.transform.SetParent(canvasObj.transform);
        linePrefab.SetActive(false);

        GameObject markerObj = new GameObject("PlayerMarker");
        markerObj.transform.SetParent(containerObj.transform);
        markerObj.AddComponent<Image>().color = Color.cyan;
        RectTransform markerRect = markerObj.GetComponent<RectTransform>();
        markerRect.sizeDelta = new Vector2(40, 40);
        markerRect.anchorMin = markerRect.anchorMax = new Vector2(0.5f, 0.5f);

        List<GraphNode> nodes = new List<GraphNode>();

        GraphNode Put(string name, int gx, int gy, GraphNode.NodeType type = GraphNode.NodeType.Normal)
        {
            Vector2 pos = new Vector2(gx * GRID, gy * GRID);
            GraphNode node = CreateNode(containerObj, name, pos, type);
            nodes.Add(node);
            return node;
        }


        GraphNode nStart = Put("Start", -8, 0);
        GraphNode nHub1 = Put("Hub1", -6, 0);
        Link(nStart, nHub1);

        GraphNode nUp1 = Put("Up1", -6, 1);
        GraphNode nUp2 = Put("Up2", -4, 1);
        GraphNode nBtn1 = Put("Btn1", -4, 2, GraphNode.NodeType.Button);

        Link(nHub1, nUp1); Link(nUp1, nUp2); Link(nUp2, nBtn1);

        GraphNode nDown1 = Put("Down1", -6, -1);
        GraphNode nDown2 = Put("Down2", -4, -1);
        Link(nHub1, nDown1);

        nBtn1.pairsToToggle.Add(new GraphNode.ConnectionPair { a = nDown1, b = nDown2 });


        GraphNode nHub2 = Put("Hub2", -2, -1);
        Link(nDown2, nHub2);

        GraphNode nBtn2 = Put("Btn2", -2, 0, GraphNode.NodeType.Button);
        Link(nHub2, nBtn2);

        GraphNode nMazeEntry = Put("MazeEntry", 0, -1);

        nBtn2.pairsToToggle.Add(new GraphNode.ConnectionPair { a = nHub2, b = nMazeEntry });


        GraphNode cur = nMazeEntry;
        int currentX = 0;
        int currentY = -1;

        for (int i = 0; i < 5; i++)
        {
            currentX += 2;
            GraphNode nNextX = Put("SnakeX_" + i, currentX, currentY);
            Link(cur, nNextX);

            currentY = (i % 2 == 0) ? 1 : -1;
            GraphNode nNextY = Put("SnakeY_" + i, currentX, currentY);
            Link(nNextX, nNextY);
            cur = nNextY;
        }

        GraphNode nPreFinal = Put("PreFinal", currentX + 2, currentY);
        Link(cur, nPreFinal);

        GraphNode nBtn3 = Put("Btn3", currentX + 2, currentY + 1, GraphNode.NodeType.Button);
        Link(nPreFinal, nBtn3);

        GraphNode nGoal = Put("CORE", currentX + 4, currentY, GraphNode.NodeType.Goal);
        nBtn3.pairsToToggle.Add(new GraphNode.ConnectionPair { a = nPreFinal, b = nGoal });

        SetPrivateField(manager, "minigameUI", bgObj);
        SetPrivateField(manager, "nodeContainer", containerRect);
        SetPrivateField(manager, "playerMarker", markerRect);
        SetPrivateField(manager, "linePrefab", linePrefab);
        manager.allNodes = nodes;

    }

    private void Link(GraphNode a, GraphNode b)
    {
        if (a == null || b == null) return;
        if (!a.neighbors.Contains(b)) a.neighbors.Add(b);
        if (!b.neighbors.Contains(a)) b.neighbors.Add(a);
    }

    private GraphNode CreateNode(GameObject parent, string name, Vector2 pos, GraphNode.NodeType type)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform);
        Image img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.2f, 0.4f, 1f);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(45, 45);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        go.AddComponent<Outline>().effectDistance = new Vector2(2, -2);
        GraphNode node = go.AddComponent<GraphNode>();
        node.type = type;
        return node;
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MinigameBuilder))]
public class MinigameBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("BUILD HACKING MINIGAME", GUILayout.Height(40))) ((MinigameBuilder)target).BuildMinigame();
    }
}
#endif

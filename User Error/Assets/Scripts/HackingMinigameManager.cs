using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HackingMinigameManager : MonoBehaviour
{
    public static HackingMinigameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject minigameUI;
    [SerializeField] private RectTransform nodeContainer;
    [SerializeField] private RectTransform playerMarker;
    [SerializeField] private GameObject linePrefab;

    [Header("Game State")]
    public List<GraphNode> allNodes = new List<GraphNode>();
    private GraphNode currentNode;
    private bool isActive = false;
    private DoorController currentDoor;

    [Header("Camera Settings")]
    [SerializeField] private float scrollSpeed = 12f;
    [SerializeField] private float moveThreshold = 0.1f;

    private List<GameObject> activeLines = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (minigameUI != null) minigameUI.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        HandleInput();
        UpdateCamera();

        if (Input.GetKeyDown(KeyCode.Escape)) EndGame(false);
    }

    private void UpdateCamera()
    {
        if (nodeContainer != null)
        {
            Vector2 targetPos = -currentNode.GetPosition();
            if (Vector2.Distance(nodeContainer.anchoredPosition, targetPos) > moveThreshold)
            {
                nodeContainer.anchoredPosition = Vector2.Lerp(nodeContainer.anchoredPosition, targetPos, Time.deltaTime * scrollSpeed);
            }
            else
            {
                nodeContainer.anchoredPosition = targetPos;
            }
        }
    }

    public void StartGame(DoorController door)
    {
        if (isActive) return;
        currentDoor = door;
        isActive = true;
        if (minigameUI != null) minigameUI.SetActive(true);

        if (allNodes.Count > 0)
        {
            currentNode = allNodes[0];
            if (nodeContainer != null) nodeContainer.anchoredPosition = -currentNode.GetPosition();
            UpdateVisuals();
        }
        LockPlayer(true);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) MoveTowards(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) MoveTowards(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveTowards(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveTowards(Vector2.right);
    }

    private void MoveTowards(Vector2 direction)
    {
        GraphNode bestNode = null;
        float bestDot = 0.9f;

        foreach (var neighbor in currentNode.neighbors)
        {
            Vector2 dirToNeighbor = (neighbor.GetPosition() - currentNode.GetPosition()).normalized;
            float dot = Vector2.Dot(dirToNeighbor, direction);

            if (dot > bestDot)
            {
                bestDot = dot;
                bestNode = neighbor;
            }
        }

        if (bestNode != null)
        {
            currentNode = bestNode;
            currentNode.OnClick();
            OnEnterNode(currentNode);
            UpdateVisuals();
        }
    }

    private void OnEnterNode(GraphNode node)
    {
        if (node.type == GraphNode.NodeType.Goal) EndGame(true);
    }

    public void UpdateVisuals()
    {
        if (playerMarker != null) playerMarker.anchoredPosition = currentNode.GetPosition();

        foreach (var line in activeLines) if (line != null) Destroy(line);
        activeLines.Clear();

        HashSet<System.Tuple<GraphNode, GraphNode>> drawnEdges = new HashSet<System.Tuple<GraphNode, GraphNode>>();

        foreach (var node in allNodes)
        {
            if (node == null) continue;
            node.SetState(node == currentNode);

            foreach (var neighbor in node.neighbors)
            {
                if (neighbor == null) continue;
                var edge = new System.Tuple<GraphNode, GraphNode>(node, neighbor);
                var rev = new System.Tuple<GraphNode, GraphNode>(neighbor, node);

                if (!drawnEdges.Contains(edge) && !drawnEdges.Contains(rev))
                {
                    DrawLine(node.GetPosition(), neighbor.GetPosition());
                    drawnEdges.Add(edge);
                }
            }
        }
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        if (linePrefab == null || nodeContainer == null) return;
        GameObject lineObj = Instantiate(linePrefab, nodeContainer);
        lineObj.SetActive(true);
        lineObj.transform.SetAsFirstSibling();
        activeLines.Add(lineObj);

        RectTransform rect = lineObj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(0, 0.5f);
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);

        Vector2 dir = (end - start);
        rect.sizeDelta = new Vector2(dir.magnitude, 8f);
        rect.anchoredPosition = start;
        rect.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    private void EndGame(bool success)
    {
        isActive = false;
        if (minigameUI != null) minigameUI.SetActive(false);
        LockPlayer(false);
        if (success && currentDoor != null) currentDoor.OpenDoor();
    }

    private void LockPlayer(bool lockIt)
    {
        Hero player = FindObjectOfType<Hero>();
        if (player != null)
        {
            player.enabled = !lockIt;
            if (lockIt)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }
        if (DialogManager.Instance != null)
        {
            if (lockIt) DialogManager.Instance.DisablePlayerControl();
            else DialogManager.Instance.EnablePlayerControl();
        }
    }
}

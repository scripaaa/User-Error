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
    [SerializeField] private float scrollSpeed = 10f;
    private Vector2 targetContainerPos;

    private List<GameObject> activeLines = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if (minigameUI != null) minigameUI.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;

        HandleInput();
        UpdateCamera();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame(false);
        }
    }

    private void UpdateCamera()
    {
        // Smoothly follow the player marker by moving the container in opposite direction
        if (nodeContainer != null)
        {
            targetContainerPos = -currentNode.GetPosition();
            nodeContainer.anchoredPosition = Vector2.Lerp(nodeContainer.anchoredPosition, targetContainerPos, Time.deltaTime * scrollSpeed);
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
        float bestDot = 0.5f;

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
            OnEnterNode(currentNode);
            UpdateVisuals();
        }
    }

    private void OnEnterNode(GraphNode node)
    {
        if (node.type == GraphNode.NodeType.Button)
        {
            foreach (var locked in node.lockedNeighbors)
            {
                if (!node.neighbors.Contains(locked))
                {
                    node.neighbors.Add(locked);
                    locked.neighbors.Add(node);
                }
            }
            node.type = GraphNode.NodeType.Normal;
            UpdateVisuals(); // Redraw lines to show new paths
        }
        else if (node.type == GraphNode.NodeType.Goal)
        {
            EndGame(true);
        }
    }

    private void UpdateVisuals()
    {
        if (playerMarker != null)
            playerMarker.anchoredPosition = currentNode.GetPosition();

        foreach (var line in activeLines) if (line != null) Destroy(line);
        activeLines.Clear();

        HashSet<System.Tuple<GraphNode, GraphNode>> drawnEdges = new HashSet<System.Tuple<GraphNode, GraphNode>>();

        foreach (var node in allNodes)
        {
            if (node == null) continue;
            bool isCurrent = (node == currentNode);
            node.SetState(isCurrent, false);

            foreach (var neighbor in node.neighbors)
            {
                if (neighbor == null) continue;
                var edge = new System.Tuple<GraphNode, GraphNode>(node, neighbor);
                var reverseEdge = new System.Tuple<GraphNode, GraphNode>(neighbor, node);

                if (!drawnEdges.Contains(edge) && !drawnEdges.Contains(reverseEdge))
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
        
        // Pivot must be (0, 0.5) for this math to work correctly
        rect.pivot = new Vector2(0, 0.5f);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        
        Vector2 dir = (end - start);
        float distance = dir.magnitude;
        
        rect.sizeDelta = new Vector2(distance, 6f);
        rect.anchoredPosition = start; // Start exactly at the node center
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rect.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void EndGame(bool success)
    {
        isActive = false;
        if (minigameUI != null) minigameUI.SetActive(false);
        
        LockPlayer(false);

        if (success && currentDoor != null)
        {
            currentDoor.ActivateGlitchWithDelay(0f);
        }
    }

    private void LockPlayer(bool lockIt)
    {
        if (DialogManager.Instance != null)
        {
            if (lockIt) DialogManager.Instance.DisablePlayerControl();
            else DialogManager.Instance.EnablePlayerControl();
        }
        else
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
        }
    }
}

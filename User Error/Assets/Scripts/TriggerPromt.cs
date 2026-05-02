using UnityEngine;
using TMPro;

public class TriggerPrompt : MonoBehaviour
{
    [Header("Prompt Settings")]
    [SerializeField] private string promptText = "Press [E]";
    [SerializeField] private float fontSize = 3f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.2f, 0);
    [SerializeField] private float activationDistance = 1.5f;

    private TextMeshPro textMesh;
    private Transform player;
    private bool isShown = false;

    void Start()
    {
        // Ищем игрока
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        // Создаем объект текста программно (как у порталов)
        GameObject textObj = new GameObject("InteractionPrompt");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = offset;

        textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.text = promptText;
        textMesh.fontSize = fontSize;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = textColor;
        
        // Делаем обводку для читаемости
        textMesh.outlineWidth = 0.2f;
        textMesh.outlineColor = Color.black;
        
        // Прячем в начале
        textMesh.gameObject.SetActive(false);
    }

    void Update()
    {
        if (player == null) return;

        // Считаем дистанцию
        float distance = Vector3.Distance(transform.position, player.position);
        bool shouldShow = distance <= activationDistance;

        if (shouldShow != isShown)
        {
            isShown = shouldShow;
            textMesh.gameObject.SetActive(isShown);
        }

        // Если текст виден, поворачиваем его к камере (чтобы не был плоским)
        if (isShown)
        {
            textMesh.transform.rotation = Quaternion.identity;
        }
    }

    // Методы для совместимости
    public void Show() { isShown = true; if(textMesh) textMesh.gameObject.SetActive(true); }
    public void Hide() { isShown = false; if(textMesh) textMesh.gameObject.SetActive(false); }
}

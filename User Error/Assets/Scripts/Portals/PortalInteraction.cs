using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortalInteraction : MonoBehaviour
{
    [Header("Настройки текста")]
    [SerializeField] private string interactionText = "Нажмите [E]";
    [SerializeField] private float yOffset = 1.5f;
    [SerializeField] private float fontSize = 3;
    [SerializeField] private TMP_FontAsset fontAsset;

    [Header("Настройки активации")]
    [SerializeField] private float activationDistance = 3f;

    private TextMeshProUGUI uiText;
    private GameObject textObject;
    private Transform player;

    private void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        textObject = new GameObject("PortalText");
        textObject.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 60);

        uiText = textObject.AddComponent<TextMeshProUGUI>();
        uiText.text = interactionText;
        uiText.fontSize = fontSize;
        uiText.alignment = TextAlignmentOptions.Center;
        uiText.color = Color.white;

        if (fontAsset != null)
        {
            uiText.font = fontAsset;
        }

        textObject.SetActive(false);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        textObject.SetActive(distance <= activationDistance);

        if (textObject.activeSelf)
        {
            Vector3 worldPos = transform.position + new Vector3(0, yOffset, 0);
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            
            RectTransform rectTransform = uiText.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = screenPos;
        }
    }

    private void OnDestroy()
    {
        if (textObject != null)
        {
            Destroy(textObject);
        }
    }
}

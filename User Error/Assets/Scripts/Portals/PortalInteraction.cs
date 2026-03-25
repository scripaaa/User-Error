using UnityEngine;
using TMPro;

public class PortalInteraction : MonoBehaviour
{
    [Header("Настройки текста")]
    [SerializeField] private string promptText = "Нажмите [E] для входа";
    [SerializeField] private float fontSize = 4;
    [SerializeField] private TMP_FontAsset fontAsset;
    [SerializeField] private Color textColor = Color.black;

    [Header("Настройки активации")]
    [SerializeField] private float activationDistance = 1.4f;

    private TextMeshPro textMesh;
    private Camera mainCamera;
    private Transform player;

    private void Start()
    {
        mainCamera = Camera.main;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            GameObject textObj = new GameObject("PortalPrompt");
            textObj.transform.SetParent(transform);
            textObj.transform.localPosition = new Vector3(0, 0.215f, 0);

            textMesh = textObj.AddComponent<TextMeshPro>();
            textMesh.text = promptText;
            textMesh.fontSize = fontSize;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.color = textColor;
            textMesh.outlineWidth = 0.2f;
            textMesh.outlineColor = Color.white;

            if (fontAsset != null)
            {
                textMesh.font = fontAsset;
            }
        }

        if (textMesh != null)
        {
            textMesh.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (textMesh == null) return;

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            textMesh.gameObject.SetActive(distance <= activationDistance);
        }

        if (textMesh.gameObject.activeSelf && mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, 
                            mainCamera.transform.rotation * Vector3.up);
        }
    }
}

using UnityEngine;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    [Header("Настройки текста")]
    [SerializeField] private string promptText = "Нажмите [E] чтобы подобрать";
    [SerializeField] private float fontSize = 2;
    [SerializeField] private Color textColor = Color.black;

    [Header("Настройки активации")]
    [SerializeField] private float activationDistance = 2f;

    [Header("Настройки масштаба")]
    [SerializeField] private float itemScale = 0.3f;

    [Header("Данные предмета")]
    [SerializeField] private ItemData itemData; 

    private TextMesh textMesh;
    private Camera mainCamera;
    private Transform player;
    private bool isCollected = false;
    [SerializeField] private GameObject portalToActivate;


    private void Start()
    {
        mainCamera = Camera.main;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        transform.localScale = Vector3.one * itemScale;

        GameObject textObj = new GameObject("ItemPrompt");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, 0.5f, 0);

        textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = promptText;
        textMesh.fontSize = (int)(fontSize * 24);
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = textColor;
        textMesh.characterSize = 0.1f;
        textMesh.GetComponent<Renderer>().sortingOrder = 100;

        textObj.SetActive(false);
    }

    private void Update()
    {
        if (isCollected) return;

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            bool inRange = distance <= activationDistance;

            if (textMesh != null)
            {
                textMesh.gameObject.SetActive(inRange);
            }

            if (inRange && Input.GetKeyDown(KeyCode.E))
            {
                CollectItem();
            }
        }

        if (textMesh != null && textMesh.gameObject.activeSelf && mainCamera != null)
        {
            textMesh.transform.LookAt(textMesh.transform.position + mainCamera.transform.forward, mainCamera.transform.up);
        }
    }

    private void CollectItem()
    {
        // Проверяем, назначен ли файл данных
        if (itemData == null)
        {
            Debug.LogError("!!! КРИТИЧЕСКАЯ ОШИБКА: На предмете " + gameObject.name + " не назначен ItemData в Инспекторе!");
            return;
        }

        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.AddItemToCell(itemData);
        }
        else
        {
            Debug.LogError("!!! ОШИБКА: InventoryManager не найден на сцене!");
        }

        if (CollectionCounter.instance != null)
        {
            CollectionCounter.instance.Collect();
            CollectionCounter.collectedItems.Add(itemData);
        }

        isCollected = true;
        if (portalToActivate != null) portalToActivate.SetActive(true);
        Destroy(gameObject);
    }
}

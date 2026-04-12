using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    // Срабатывает каждый раз, когда ты открываешь инвентарь
    private void OnEnable()
    {
        RefreshVisuals();
        RefreshIcons();
    }

    // Добавим старт на всякий случай
    private void Start()
    {
        RefreshVisuals();
        RefreshIcons();

    }

    public void RefreshIcons()
    {
        // cellIcons — List<Image> из InventoryManager
        // просто заполни по списку
        if (InventoryManager.instance == null) return;

        InventoryManager.instance.RedrawFromList(CollectionCounter.collectedItems);
    }

    public void RefreshVisuals()
    {
        // Если "вечный" счетчик существует, берем у него число
        if (CollectionCounter.instance != null && counterText != null)
        {
            counterText.text = $"{CollectionCounter.instance.Count}/5";
            Debug.Log("Инвентарь обновил текст: " + counterText.text);
        }
    }

    public void ToggleInventory()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf) RefreshVisuals();
    }
}
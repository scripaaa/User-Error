using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    private void OnEnable()   // это всё равно пригодится при открытии инвентаря
    {
        RefreshVisuals();
        RefreshIcons();
    }

    // Start можно оставить пустым или удалить, если обновление идёт через OnEnable и событие сцены
    private void Start()
    {
        // теперь не критично
    }

    public void RefreshVisuals()
    {
        if (CollectionCounter.instance != null && counterText != null)
        {
            counterText.text = $"{CollectionCounter.instance.Count}/5";
        }
    }

    public void RefreshIcons()
    {
        if (InventoryManager.instance != null)
            InventoryManager.instance.RedrawFromList(CollectionCounter.collectedItems);
    }

    public void ToggleInventory()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        if (gameObject.activeSelf) RefreshVisuals(); // уже есть OnEnable
    }
}
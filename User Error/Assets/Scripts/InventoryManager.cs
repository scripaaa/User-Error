using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    [SerializeField] private List<Image> cellIcons;

    private void Awake()
    {
        instance = this;
    }

    public void AddItemToCell(ItemData itemToAdd)
    {
        if (itemToAdd == null) return;

        for (int i = 0; i < cellIcons.Count; i++)
        {
            // Проверяем именно по активности объекта — надёжно и просто
            if (!cellIcons[i].gameObject.activeSelf)
            {
                cellIcons[i].sprite = itemToAdd.itemIcon;
                cellIcons[i].color = Color.white;
                cellIcons[i].gameObject.SetActive(true);

                Debug.Log($"<color=cyan>Инвентарь:</color> добавлен {itemToAdd.itemName} в слот {i}");
                return;
            }
        }

        Debug.LogWarning("Инвентарь полон!");
    }

    public void RedrawFromList(List<ItemData> items)
    {
        // сначала всё очищаем
        foreach (var icon in cellIcons)
            icon.gameObject.SetActive(false);

        // потом заполняем
        for (int i = 0; i < items.Count && i < cellIcons.Count; i++)
        {
            cellIcons[i].sprite = items[i].itemIcon;
            cellIcons[i].color = Color.white;
            cellIcons[i].gameObject.SetActive(true);
        }
    }
}
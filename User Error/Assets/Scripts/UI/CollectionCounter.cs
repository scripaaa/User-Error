using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CollectionCounter : MonoBehaviour
{
    public static CollectionCounter instance;

    // Сделаем их static, чтобы они железно жили между сценами
    private static int totalSavedCount = 0;
    private static int countInCurrentLevel = 0;
    public static List<ItemData> collectedItems = new List<ItemData>();
    public int Count => totalSavedCount + countInCurrentLevel;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Загружаем данные. Если в меню мы удалили ключ, загрузится 0.
            totalSavedCount = PlayerPrefs.GetInt("TotalCollectedItems", 0);
            countInCurrentLevel = 0;
        }
        else
        {
            // Если мы вернулись в меню и зашли снова, старый объект выживет, 
            // но нам нужно обнулить его данные, так как это "Новая игра"
            instance.ResetData();
            Destroy(gameObject);
            return;
        }
    }

    public void ResetData()
    {
        totalSavedCount = 0;
        countInCurrentLevel = 0;
        collectedItems.Clear();
        UpdateAllUI();
    }

    public void Collect()
    {
        countInCurrentLevel++;
        Debug.Log("<color=green>ПРЕДМЕТ СОБРАН!</color> Текущий счет: " + Count);

        // Пытаемся обновить инвентарь, если он сейчас активен на сцене
        InventoryUI ui = Object.FindAnyObjectByType<InventoryUI>(FindObjectsInactive.Include);
        if (ui != null) ui.RefreshVisuals();
    }

    public void UpdateAllUI()
    {
        // Ищем текст по тегу. ВАЖНО: Тег должен быть "ItemCounterText" (с Т на конце!)
        GameObject textObj = GameObject.FindWithTag("ItemCounterText");

        if (textObj != null)
        {
            TextMeshProUGUI uiText = textObj.GetComponent<TextMeshProUGUI>();
            if (uiText != null)
            {
                uiText.text = $"{Count}/5";
            }
        }
    }

    public void ResetProgressFull()
    {
        totalSavedCount = 0;
        countInCurrentLevel = 0;
        PlayerPrefs.DeleteKey("TotalCollectedItems");
        PlayerPrefs.Save();

        // Сразу пытаемся обновить UI, если мы уже на уровне
        UpdateAllUI();
        Debug.Log("<color=red>ПРОГРЕСС ПОЛНОСТЬЮ ОБНУЛЕН</color>");
    }
}
using UnityEngine;
using TMPro;

public class CollectionCounter : MonoBehaviour
{
    public static CollectionCounter instance;

    [SerializeField] private TextMeshProUGUI counterText;
    private int collectedCount = 0;
    private int maxCount = 5;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void Collect()
    {
        collectedCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (counterText != null)
        {
            counterText.text = $"{collectedCount}/{maxCount}";
        }
    }
}

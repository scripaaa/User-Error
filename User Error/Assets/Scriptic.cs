using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private Hero playerHero;

    private void OnEnable()
    {
        UpdateCounter();
    }

    public void UpdateCounter()
    {
        if (playerHero != null && counterText != null)
        {
            counterText.text = $"{playerHero.countCollectedItems}/5";
        }
    }
}
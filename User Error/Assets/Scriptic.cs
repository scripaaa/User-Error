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

    public void ToggleInventory()
    {
        bool isActive = gameObject.activeSelf;
        gameObject.SetActive(!isActive);
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }
}
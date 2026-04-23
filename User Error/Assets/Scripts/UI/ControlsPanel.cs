using UnityEngine;
using UnityEngine.UI;

public class ControlsPanel : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Image[] controlImages;

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Hide);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

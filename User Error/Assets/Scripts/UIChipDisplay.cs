using UnityEngine;
using TMPro;

public class UIChipDisplay : MonoBehaviour
{
    public static UIChipDisplay Instance;

    public TextMeshProUGUI chipText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateUI(int count)
    {
        chipText.text = count.ToString() + "/3";
    }
}

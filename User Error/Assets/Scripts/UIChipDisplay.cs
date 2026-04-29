using UnityEngine;
using TMPro;

public class UIChipDisplay : MonoBehaviour
{
    public TMP_Text chipText;

    void Update()
    {
        int current = ChipManager.Instance.chips;

        if (current >= 3)
        {
            chipText.text = "»дите к панели";
        }
        else
        {
            chipText.text = current + "/3";
        }
    }
}

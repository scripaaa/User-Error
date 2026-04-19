using UnityEngine;

public class ChipManager : MonoBehaviour
{
    public static ChipManager Instance;

    public int chipsCollected = 0;
    public int chipsRequired = 3;

    private void Awake()
    {
        Instance = this;
    }

    public void AddChip()
    {
        chipsCollected++;
        UIChipDisplay.Instance.UpdateUI(chipsCollected);
    }

    public bool HasAllChips()
    {
        return chipsCollected >= chipsRequired;
    }
}

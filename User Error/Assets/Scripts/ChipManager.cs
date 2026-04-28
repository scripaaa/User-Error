using UnityEngine;

public class ChipManager : MonoBehaviour
{
    public static ChipManager Instance;

    public int chips = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void AddChip()
    {
        chips++;
    }

    public bool HasAllChips()
    {
        return chips >= 3;
    }
}

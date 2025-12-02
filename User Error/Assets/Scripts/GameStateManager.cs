using UnityEngine;

public static class GameStateManager
{
    public static string SpawnPointName { get; set; }
    public static bool CameFromPortal { get; set; }

    // Опционально: метод для сброса состояния
    public static void ResetPortalState()
    {
        SpawnPointName = null;
        CameFromPortal = false;
    }
}
using UnityEngine;

/// <summary>
/// Менеджер, отвечающий только за чек‑поинты текущего уровня.
/// Хранит позицию последнего активированного чекпоинта и умеет
/// возвращать игрока в эту точку при смерти.
/// </summary>
public class LevelCheckpointManager : MonoBehaviour
{
    // Текущая позиция чекпоинта. По умолчанию – стартовая позиция игрока.
    private Vector3 _currentCheckpoint;

    // Ссылка на игрока (Hero) – будет найдена в Awake.
    private Hero _hero;

    #region Public API

    /// <summary>
    /// Устанавливает новую позицию чекпоинта.
    /// </summary>
    public void SetCheckpoint(Vector3 newPos)
    {
        _currentCheckpoint = newPos;
        // Можно добавить визуальный/звуковой отклик, например:
        // Debug.Log($"[LevelCheckpointManager] Checkpoint set at {newPos}");
    }

    /// <summary>
    /// Перемещает героя в последнюю сохранённую точку.
    /// Вызывается из Hero.Die() или любого другого места.
    /// </summary>
    public void RespawnHero()
    {
        if (_hero == null)
        {
            Debug.LogWarning("[LevelCheckpointManager] Hero reference is missing, cannot respawn.");
            return;
        }

        _hero.transform.position = _currentCheckpoint;
        // Если у героя есть восстановление здоровья/энергии – вызовите его здесь
        // _hero.ResetHealth();
    }

    #endregion

    private void Awake()
    {
        // Находим героя в сцене один раз.
        _hero = FindObjectOfType<Hero>();
        if (_hero == null)
        {
            Debug.LogError("[LevelCheckpointManager] Hero not found in the scene!");
            return;
        }

        // Начальная точка спавна – позиция героя в момент старта уровня.
        _currentCheckpoint = _hero.transform.position;
    }

    // Опционально: можно сделать синглтон, если удобно обращаться к нему из любого скрипта.
    public static LevelCheckpointManager Instance { get; private set; }

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[LevelCheckpointManager] More than one instance found! " +
                             "Only the first will be used.");
            return;
        }
        Instance = this;
    }

    private void OnDisable()
    {
        if (Instance == this) Instance = null;
    }
}

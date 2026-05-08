using UnityEngine;

public class ShowPCCanvas : MonoBehaviour
{
    [Header("Canvas to Show/Hide")]
    [SerializeField] private GameObject targetCanvas; // Ваш Canvas

    [Header("Hero Settings (auto-find if empty)")]
    [SerializeField] private Hero hero;               // Можно перетянуть вручную, можно оставить пустым – найдёт сам

    private Rigidbody2D heroRigidbody;
    private bool isCanvasActive = false;
    private Vector2 savedVelocity;       // на случай, если хотим восстановить скорость
    private bool wasKinematic;

    private void Start()
    {
        // Поиск героя, если не назначен вручную
        if (hero == null)
        {
            hero = FindObjectOfType<Hero>();
            if (hero == null)
                Debug.LogError("[CanvasInteraction] Не найден компонент Hero на сцене!");
        }

        // Получаем Rigidbody2D у героя
        if (hero != null)
            heroRigidbody = hero.GetComponent<Rigidbody2D>();

        // Скрываем Canvas при старте
        if (targetCanvas != null)
            targetCanvas.SetActive(false);

        // Стартовое состояние курсора (подстройте под свою игру)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleCanvasMode();
        }
    }

    private void ToggleCanvasMode()
    {
        if (hero == null) return;

        isCanvasActive = !isCanvasActive;

        if (isCanvasActive)
        {
            // ----- ВКЛЮЧАЕМ РЕЖИМ CANVAS (пауза движения) -----
            // 1. Показываем Canvas
            if (targetCanvas != null)
                targetCanvas.SetActive(true);

            // 2. Останавливаем физическое движение
            if (heroRigidbody != null)
            {
                savedVelocity = heroRigidbody.linearVelocity;
                heroRigidbody.linearVelocity = Vector2.zero;
                heroRigidbody.simulated = false;   // полная заморозка физики
                wasKinematic = heroRigidbody.isKinematic;
                heroRigidbody.isKinematic = true;   // чтобы никакие силы не двигали
            }

            // 3. Отключаем компонент Hero (перестанут вызываться его Update/FixedUpdate)
            hero.enabled = false;

            // 4. Курсор – для взаимодействия с UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // ----- ВЫКЛЮЧАЕМ РЕЖИМ CANVAS (возвращаем управление) -----
            // 1. Прячем Canvas
            if (targetCanvas != null)
                targetCanvas.SetActive(false);

            // 2. Возвращаем физику
            if (heroRigidbody != null)
            {
                heroRigidbody.simulated = true;
                heroRigidbody.isKinematic = wasKinematic;
                // Восстанавливаем скорость, если нужно (или оставляем ноль)
                heroRigidbody.linearVelocity = savedVelocity;
            }

            // 3. Включаем Hero обратно
            hero.enabled = true;

            // 4. Возвращаем курсор в игровой режим (подстройте под свою игру)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

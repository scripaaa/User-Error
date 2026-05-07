using UnityEngine;

public class MonitorUIManager : MonoBehaviour
{
    [Header("Окна монитора")]
    public GameObject mainPanel;    // главное меню (с кнопками выбора разделов)
    public GameObject panel1;       // раздел 1
    public GameObject panel2;       // раздел 2
    public GameObject panel3;       // раздел 3

    private bool isMonitorOpen = false;

 

    void Update()
    {
        // Открыть/закрыть по E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isMonitorOpen)
                CloseMonitor();
            else
                OpenMonitor();
        }

        // Закрыть монитор по Escape
        if (isMonitorOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMonitor();
        }
    }

    // Открыть монитор – показываем главное меню
    public void OpenMonitor()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        // Скрываем все панели на всякий случай
        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(false);
        if (panel3 != null) panel3.SetActive(false);

        Time.timeScale = 0f;
        isMonitorOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Полностью закрыть монитор
    public void CloseMonitor()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(false);
        if (panel3 != null) panel3.SetActive(false);

        Time.timeScale = 1f;
        isMonitorOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Кнопка MAIN MENU – возвращает в главное меню из любого раздела
    public void GoToMainMenu()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(false);
        if (panel3 != null) panel3.SetActive(false);
    }

    // Показать раздел 1
    public void ShowPanel1()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (panel1 != null) panel1.SetActive(true);
        if (panel2 != null) panel2.SetActive(false);
        if (panel3 != null) panel3.SetActive(false);
    }

    // Показать раздел 2
    public void ShowPanel2()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(true);
        if (panel3 != null) panel3.SetActive(false);
    }

    // Показать раздел 3
    public void ShowPanel3()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (panel1 != null) panel1.SetActive(false);
        if (panel2 != null) panel2.SetActive(false);
        if (panel3 != null) panel3.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class MonitorUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;

    [Header("Hero")]
    public Hero hero;

    private bool isMonitorOpen;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleMonitor();

        if (isMonitorOpen && Input.GetKeyDown(KeyCode.Escape))
            CloseMonitor();
    }

    void ToggleMonitor()
    {
        if (isMonitorOpen)
            CloseMonitor();
        else
            OpenMonitor();
    }

    // ======================
    // OPEN
    // ======================

    public void OpenMonitor()
    {
        isMonitorOpen = true;

        ShowOnly(mainPanel);

        if (hero != null)
            hero.CanControl = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventSystem.current.SetSelectedGameObject(null);
    }

    // ======================
    // CLOSE
    // ======================

    public void CloseMonitor()
    {
        isMonitorOpen = false;

        ShowOnly(null);

        if (hero != null)
        {
            hero.CanControl = true;

            hero.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EventSystem.current.SetSelectedGameObject(null);
    }

    // ======================
    // PANELS
    // ======================

    void ShowOnly(GameObject target)
    {
        if (mainPanel) mainPanel.SetActive(target == mainPanel);
        if (panel1) panel1.SetActive(target == panel1);
        if (panel2) panel2.SetActive(target == panel2);
        if (panel3) panel3.SetActive(target == panel3);
    }

    public void GoToMainMenu() => ShowOnly(mainPanel);
    public void ShowPanel1() => ShowOnly(panel1);
    public void ShowPanel2() => ShowOnly(panel2);
    public void ShowPanel3() => ShowOnly(panel3);
}
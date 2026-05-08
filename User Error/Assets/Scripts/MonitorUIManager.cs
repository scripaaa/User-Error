using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MonitorUIManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public GameObject panel4;


    private bool isOpen;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ToggleMonitor();

        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
            CloseMonitor();
    }

    void ToggleMonitor()
    {
        if (isOpen)
            CloseMonitor();
        else
            OpenMonitor();
    }

    // ======================
    // OPEN
    // ======================

    public void OpenMonitor()
    {
        isOpen = true;

        ShowOnly(mainPanel);

        // ⭐ СТАВИМ ИГРУ НА ПАУЗУ
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        EventSystem.current.SetSelectedGameObject(null);
    }

    // ======================
    // CLOSE
    // ======================

    public void CloseMonitor()
    {
        isOpen = false;

        ShowOnly(null);

        // ⭐ ВОЗВРАЩАЕМ ИГРУ
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        EventSystem.current.SetSelectedGameObject(null);

        SceneManager.LoadScene("SecondFinall");
    }

    // ======================

    void ShowOnly(GameObject target)
    {
        if (mainPanel) mainPanel.SetActive(target == mainPanel);
        if (panel1) panel1.SetActive(target == panel1);
        if (panel2) panel2.SetActive(target == panel2);
        if (panel3) panel3.SetActive(target == panel3);
        if (panel4) panel4.SetActive(target == panel4);

    }

    public void GoToMainMenu() => ShowOnly(mainPanel);
    public void ShowPanel1() => ShowOnly(panel1);
    public void ShowPanel2() => ShowOnly(panel2);
    public void ShowPanel3() => ShowOnly(panel3);
    public void ShowPanel4() => ShowOnly(panel4);


    public void OpenPanelByProgress()
    {
        if (CollectionCounter.instance == null)
        {
            ShowOnly(panel1);
            return;
        }

        int collected = CollectionCounter.instance.Count;

        if (collected >= 5)
        {
            ShowOnly(panel1); // все предметы собраны
        }
        else
        {
            ShowOnly(panel2); // предметов мало
        }
    }

    public void GetAccess()
    {
        

        SceneManager.LoadScene("FinalScene");
    }

}
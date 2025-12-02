using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool PauseGame;
    public GameObject pauseGameMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseGame)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseGameMenu.SetActive(false);
        Time.timeScale = 1f;
        PauseGame = false;
    }

    public void Pause()
    {
        pauseGameMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
    }
    public void Restart()
    {
        // 1. Устанавливаем нормальную скорость игры, чтобы избежать зависания
        // (на случай, если кнопка нажата в режиме паузы)
        Time.timeScale = 1f;
        
        // 2. Загружаем текущую сцену заново по её индексу
        // SceneManager.GetActiveScene().buildIndex возвращает индекс активной сцены
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void LosdMenu()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene("Menu");
}
}
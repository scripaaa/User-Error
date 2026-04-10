using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        // 1. Очищаем физическую память (на случай если объекта еще нет)
        PlayerPrefs.DeleteKey("TotalCollectedItems");
        PlayerPrefs.Save();

        // 2. Если объект уже "приехал" из прошлой игры, сбрасываем его статические переменные
        if (CollectionCounter.instance != null)
        {
            CollectionCounter.instance.ResetProgressFull();
        }

        // 3. Загружаем игру
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Debug.Log("Игра закрылась");
        Application.Quit();
    }

    public void Exit()
    {
        ExitGame();
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level3Controller : MonoBehaviour
{
    [Header("Dialog Settings")]
    private string[] dialoglines;
    private GameObject portal;
    private GameObject Mita;


    private bool dialogShown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mita = GameObject.Find("Mita");
        Mita.gameObject.SetActive(false);
       
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator CompleteStartSequence()
    {

        dialoglines = new string[] { "О, развилка!", "Я бы на твоём месте выбрала верхний путь.", "Почему?", "Просто поверь мне, хорошо?", "Пойдём!" };

        Mita.gameObject.SetActive(true);
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.5f);
        Mita.gameObject.SetActive(false);



    }
    // Новый метод для запуска диалога и ожидания его завершения
    IEnumerator StartDialogAndWait(string[] lines)
    {
        // Ждем, пока диалоговый менеджер будет доступен
        while (DialogManager.Instance == null)
        {
            yield return null;
        }

        // Запускаем диалог
        DialogManager.Instance.StartDialog(lines);

        // Ждем, пока диалог активен
        while (DialogManager.Instance.IsDialogActive())
        {
            yield return null;
        }
    }
}

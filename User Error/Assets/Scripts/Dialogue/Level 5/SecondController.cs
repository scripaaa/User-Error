using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SecondController : MonoBehaviour
{
    [Header("Dialog Settings")]
    private string[] dialoglines;
    private GameObject Mita;


    private bool dialogShown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mita = GameObject.Find("Mita2");
        Mita.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator CompleteStartSequence()
    {


        Mita.gameObject.SetActive(true);
        dialoglines = new string[] { "Ура! Ты собрал все чипы!", "Какой же ты молодец! Я знала, что у тебя получится.", "Ну что, давай скорее посмотрим, что за этой дверью!"};
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

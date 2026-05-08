using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SecondCLevel4 : MonoBehaviour
{
    [Header("Dialog Settings")]
    private string[] dialoglines;
    private GameObject Mita;
    private GameObject Arrow;


    private bool dialogShown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mita = GameObject.Find("Mita2");
        Mita.gameObject.SetActive(false);
        Arrow = GameObject.Find("Arrow");
        Arrow.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator CompleteStartSequence()
    {


        Mita.gameObject.SetActive(true);
        dialoglines = new string[] { "Ой-ой-ой...", "Похоже, это тупик.", "Хм-м...", "А что, если попробовать просто... ударить по ней?", " Ну, знаешь, иногда грубая сила — лучшее решение. Тем более стена выглядит не очень прочной.", "Давай, разнеси её!" };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.5f);
        Mita.gameObject.SetActive(false);
        Arrow.gameObject.SetActive(true);



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

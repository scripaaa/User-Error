using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelFirstController : MonoBehaviour
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
        portal = GameObject.Find("Portal_To_PortalScene1");
        portal.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public IEnumerator CompleteStartSequence()
    {
        
        dialoglines = new string[] { "Ой...", "Извини, извини, я тут целый сектор забыла прорисовать." ,"Совсем вышла из практики... Давай-ка исправлю!" };

       Mita.gameObject.SetActive(true);
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        portal.gameObject.SetActive(true);
       
        yield return new WaitForSeconds(0.5f);

        // Меняем реплики и запускаем второй диалог
        dialoglines = new string[] { "Вот, держи короткий путь." ,"Должно быть весело!", "Удачки!~"};

        // Запускаем второй диалог и ждем его завершения
        yield return StartCoroutine(StartDialogAndWait(dialoglines));

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

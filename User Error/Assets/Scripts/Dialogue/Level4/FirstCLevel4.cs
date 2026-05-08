using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FirstCLevel4 : MonoBehaviour
{
    [Header("Dialog Settings")]
    private string[] dialoglines;
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


        Mita.gameObject.SetActive(true);
        dialoglines = new string[] { "Ну знаешь ли!", "Я же тебе ясно сказала — не ходи туда.", "А ты всё равно пошёл.",
        "Тот путь... он же забагованный. Уровень там сыпался прямо на ходу.", "Я как раз собиралась его починить, но ты решил не ждать?", "Надеюсь...","Ты не нашёл там никаких глюков?",
        "Ничего странного?","Никаких лишних предметов или... искажений?","Потому что такие вещи могут навредить... ну, игре. Мне.", "Пожалуйста, больше так не делай, хорошо? Просто доверяй мне иногда.", "Ладно, проехали."
        ,"Ты здесь, я здесь. Давай дальше...","Но уже по моим указателям, договорились?"};
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

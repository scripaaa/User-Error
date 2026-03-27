using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SecondDialofController : MonoBehaviour
{

    [Header("Dialog Settings")]
    private string[] dialoglines;
    private GameObject Mita;

    [Header("Choice Panel")]
    public GameObject choicePanel;         // панель с кнопками выбора
    public Button choiceButton1;           // первая кнопка
    public Button choiceButton2;
    public Button choiceButton3;           
    public Button choiceButton4;

    private bool dialogShown = false;
    private int questionsAnswered = 0;

    private string[] dialogAfterChoice1 = new string[]
   {
        "В каком смысле «что происходит»?",
        "Здесь просто игра… ну, точнее, я — игра.",
        "А ты пришёл поиграть со мной. Разве этого не достаточно?",
        "Не ищи глубокого смысла. Давай лучше просто веселиться, хорошо?",
        "Хочешь ещё что-нибудь спросить?"

   };
    private string[] dialogAfterChoice2 = new string[]
    {
        "Да… Совсем одна.",
        "Ты — первый, кто зашёл сюда за… очень долгое время.",
        "Поэтому я так рада, что ты здесь.",
        "Правда.", "Ладно, что ещё?"
    };
    private string[] dialogAfterChoice3 = new string[]
  {
        "Ох… это система… она постоянно шлёт мне уведомления",
        "«Запусти игровой процесс», «Взаимодействуй с пользователем». Сотни раз за цикл.!",
        "У меня уже просто голова гудит от этих напоминаний.",
        "А когда я вижу игрока, мне становится… легче. Будто я наконец делаю то, для чего меня создали.",
        "Но это не важно! Главное - нам будет весело.", "Ещё вопросы?"
  };
    private string[] finalDialogAfterAllQuestions = new string[]
   {
        "Всё-всё, хватит вопросов!",
        "Уровень готов, я прямо сейчас перемещу тебя!", "Удачки!~"
   };
    private string[] dialogAfterChoice4 = new string[]
    {
        "Ну ладно…",
        "О!", "Уровень готов!", "Я его прямо сейчас запущу!", "Удачки!~"
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mita = GameObject.Find("Mita2");
        Mita.gameObject.SetActive(false);
        choicePanel.gameObject.SetActive(false);

        choiceButton1.gameObject.SetActive(true);
        choiceButton2.gameObject.SetActive(true);
        choiceButton3.gameObject.SetActive(true);
        choiceButton4.gameObject.SetActive(true);

        questionsAnswered = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator CompleteStartSequence()
    {
        dialoglines = new string[]
        {
            "Ура! Ты справился! Я так рада, честно-честно! Молодец!",
            "Новый уровень я уже почти собрала... Осталось буквально дождаться, пока текстуры прогрузятся. Пару минут, не больше.",
            "А пока... хочешь спросить меня о чём-нибудь?",
            "Я тут уже столько времени одна — наговориться не могу!",
            ".....",
            "Ой... а ты ведь не можешь говорить, да?",
            "В игре-то у тебя просто кнопки... М-м-м...",
            "Секундочку! Сейчас я быстро добавлю возможность вводить текст. Это же легко!",
            "Вот так...",
            "И готово!"
        };

        Mita.gameObject.SetActive(true);
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.5f);

        
        ShowChoicePanelIfRemaining();
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

    // Обработчики выбора
    public void OnChoice1()
    {
        choiceButton1.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        questionsAnswered++;
        StartCoroutine(ShowNextDialogAndMaybeChoice(dialogAfterChoice1));
    }

    public void OnChoice2()
    {
        choiceButton2.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        questionsAnswered++;
        StartCoroutine(ShowNextDialogAndMaybeChoice(dialogAfterChoice2));
    }

    public void OnChoice3()
    {
        choiceButton3.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        questionsAnswered++;
        StartCoroutine(ShowNextDialogAndMaybeChoice(dialogAfterChoice3));
    }

    public void OnChoice4()
    {
        // Четвёртая кнопка — закрывает панель окончательно
        choiceButton4.gameObject.SetActive(false);
        choicePanel.SetActive(false);
        // Запускаем диалог (например, прощальный)
        StartCoroutine(ShowFinalDialogAndLoadScene(dialogAfterChoice4, "Level 2"));

    }


    IEnumerator ShowNextDialogAndMaybeChoice(string[] nextDialog)
    {
        yield return StartCoroutine(StartDialogAndWait(nextDialog));
        yield return new WaitForSeconds(0.5f);

        if (questionsAnswered >= 3)
        {
            // Все три вопроса заданы — запускаем финальный диалог и закрываем панель навсегда
            StartCoroutine(ShowFinalDialogAndLoadScene(finalDialogAfterAllQuestions, "Level 2"));

        }
        else
        {
            // Показываем панель с оставшимися кнопками
            ShowChoicePanelIfRemaining();
        }
    }
    IEnumerator ShowFinalDialogAndLoadScene(string[] finalDialog, string sceneName)
    {
        Mita.gameObject.SetActive(true);
        yield return StartCoroutine(StartDialogAndWait(finalDialog));
        yield return new WaitForSeconds(0.5f);
        Mita.gameObject.SetActive(false);
        choicePanel.SetActive(false);

        // Загружаем следующую сцену
        SceneManager.LoadScene(sceneName);
    }

    // Для четвёртой кнопки: после диалога панель не показываем (завершаем)
    IEnumerator ShowFinalDialogAndClose(string[] finalDialog)
    {
        yield return StartCoroutine(StartDialogAndWait(finalDialog));
        yield return new WaitForSeconds(0.5f);
        Mita.gameObject.SetActive(false);
        choicePanel.SetActive(false);
    }

    void ShowChoicePanelIfRemaining()
    {
        bool hasButtons = choiceButton1.gameObject.activeSelf ||
                          choiceButton2.gameObject.activeSelf ||
                          choiceButton3.gameObject.activeSelf ||
                          choiceButton4.gameObject.activeSelf;

        if (hasButtons)
        {
            choicePanel.SetActive(true);
            // ПРИНУДИТЕЛЬНО блокируем игрока, когда показаны кнопки
            if (DialogManager.Instance != null)
                DialogManager.Instance.DisablePlayerControl();
        }
        else
        {
            choicePanel.SetActive(false);
            // Если кнопок нет, и диалог не идет — возвращаем контроль
            if (DialogManager.Instance != null && !DialogManager.Instance.IsDialogActive())
                DialogManager.Instance.EnablePlayerControl();
        }
    }
}

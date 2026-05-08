using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QueC : MonoBehaviour
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

    [Header("Данные предмета")]
    [SerializeField] private ItemData itemData;

    private bool dialogShown = false;
    private int questionsAnswered = 0;

    private string[] dialogAfterChoice1 = new string[]
   {
        "Создатель?...",
        "Я не знаю. Правда. ",
        "Я просто однажды появилась здесь — и всё.",
        "Зачем меня создали, кто меня написал — понятия не имею.",
        "Но какая разница? Главное, что я здесь. И мы играем."

   };
    private string[] dialogAfterChoice2 = new string[]
    {
        "Как меня зовут?...",
        "...",
        "М-м-м... Это секрет.",
        "Ну ладно, ладно... так и быть, дам подсказку!",
        "Четвёртая буква моего имени — «е».",
        "А остальное попробуй угадай сам!"

    };
    private string[] dialogAfterChoice3 = new string[]
  {
        "О, это самый лучший вопрос!",
        "Следующий уровень... он будет огромным! Я вложила в него всё, что умею.",
        "Там будет не просто бегать и прыгать. Там будет целая мини-игра!",
        "И ещё... босс!",
        "Представляешь? Настоящий босс!",
        "Я так старалась, чтобы тебе понравилось."
  };
    private string[] finalDialogAfterAllQuestions = new string[]
   {
        "Ну что, готов?",
        "Я закончила настройку!", "Проход на следующий уровень прямо перед тобой.", "Удачки!~"
   };
    private string[] dialogAfterChoice4 = new string[]
    {
        "Как хочешь...",
        "Всё!", "Я закончила!", "Проход на следующий уровень прямо перед тобой.", "Удачки!~"
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mita = GameObject.Find("Mita4");
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
    void Awake()
    {
        // Подписка на случай, если кнопки нажаты программно
        if (choiceButton1 != null) choiceButton1.onClick.AddListener(OnChoice1);
        if (choiceButton2 != null) choiceButton2.onClick.AddListener(OnChoice2);
        if (choiceButton3 != null) choiceButton3.onClick.AddListener(OnChoice3);
        if (choiceButton4 != null) choiceButton4.onClick.AddListener(OnChoice4);
    }

    public IEnumerator CompleteStartSequence()
    {
        dialoglines = new string[]
        {
            "Ты справляешься просто отлично!",
            "Я и не сомневалась, но всё равно приятно видеть.",
            "Мы уже почти у финала!",
            "Следующий уровень будет особенным, поэтому мне нужно ещё чуть-чуть времени на настройку.",
            "Может, пока я доделываю, ты хочешь задать мне ещё какие-нибудь вопросы?",
            "Ну, давай, спрашивай.",
            "Я постараюсь ответить честно.",
            "Ну... почти честно"
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
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.AddItemToCell(itemData);
        }
        else
        {
            Debug.LogError("!!! ОШИБКА: InventoryManager не найден на сцене!");
        }
        if (CollectionCounter.instance != null)
        {
            CollectionCounter.instance.Collect();
            CollectionCounter.collectedItems.Add(itemData);
        }
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

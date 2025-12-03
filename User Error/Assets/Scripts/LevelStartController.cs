using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Dialog Settings")]
    private string[] dialoglines;
    [SerializeField] private float delayBeforeDialogue = 10f;

    private GameObject backgroundParent;
    private GameObject Mita;
    private List<Image> backgroundImages = new List<Image>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mita = GameObject.Find("Mita");
        backgroundParent = GameObject.Find("BackGround");
        Debug.Log($"Найден объект BackGround, дочерних объектов: {(backgroundParent != null ? backgroundParent.transform.childCount : 0)}");
        Mita.gameObject.SetActive(false);
        if (backgroundParent != null)
        {
            Image[] images = backgroundParent.GetComponentsInChildren<Image>(true);  // Добавил true для поиска неактивных объектов
            backgroundImages.AddRange(images);
            Debug.Log($"Найдено изображений: {backgroundImages.Count}");
        }

        // Запускаем всю последовательность
        StartCoroutine(CompleteStartSequence());
    }

    IEnumerator CompleteStartSequence()
    {
        // Проверяем, что есть достаточно изображений
        if (backgroundImages.Count > 4)
        {
            // Включаем фон
            backgroundImages[4].gameObject.SetActive(true);
            Debug.Log("Фон включен");
        }
        else
        {
            Debug.LogError("Недостаточно элементов в backgroundImages");
        }

        // Ждем 3 секунды
        yield return new WaitForSeconds(3f);

        // Запускаем первый диалог
        dialoglines = new string[] { "Подожди секунду.....", "Сейчас я включу свет!", "И вот!" };

        // Ждем, пока можно запустить диалог
        yield return StartCoroutine(StartDialogAndWait(dialoglines));

        Debug.Log("Первый диалог завершен");

        // Выключаем фон
        if (backgroundImages.Count > 4)
        {
            backgroundImages[4].gameObject.SetActive(false);
            Debug.Log("Фон выключен");
            Mita.gameObject.SetActive(true);
        }

        // Небольшая пауза между диалогами
        yield return new WaitForSeconds(0.5f);

        // Меняем реплики и запускаем второй диалог
        dialoglines = new string[] { "Привет!", "Проверкааовфылдаоывжа", "лпфвалплыоавпжывап" };

        // Запускаем второй диалог и ждем его завершения
        yield return StartCoroutine(StartDialogAndWait(dialoglines));

        Debug.Log("Второй диалог завершен");

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Level 1");

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

    // Update is called once per frame
    void Update()
    {
        // Для тестирования - запуск диалога по нажатию клавиши
        if (Input.GetKeyDown(KeyCode.T))
        {
            dialoglines = new string[] { "Тестовый диалог", "Вторая строка", "Третья строка" };
            StartCoroutine(StartDialogAndWait(dialoglines));
        }
    }

    IEnumerator StartDialogAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeDialogue);

        if (DialogManager.Instance != null)
        {
            DialogManager.Instance.StartDialog(dialoglines);
        }
    }

    // Упрощенный метод для запуска диалога
    public void StartDialogueManually(string[] lines = null)
    {
        if (lines != null)
        {
            dialoglines = lines;
        }

        if (DialogManager.Instance != null && dialoglines != null && dialoglines.Length > 0)
        {
            DialogManager.Instance.StartDialog(dialoglines);
        }
    }
}

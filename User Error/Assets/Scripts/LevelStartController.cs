using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Dialog Settings")]
    private string[] dialoglines;
    [SerializeField] private float delayBeforeDialogue = 10f;

    private bool dialogStarted = false;

    private GameObject backgroundParent; // Родительский объект "Background"
    private List<Image> backgroundImages = new List<Image>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backgroundParent = GameObject.Find("BackGround");
        Debug.Log($"Найден объект BackGround, дочерних объектов: {backgroundParent.transform.childCount}");

        if (backgroundParent != null)
        {
            Image[] images = backgroundParent.GetComponentsInChildren<Image>();
            backgroundImages.AddRange(images);
            Debug.Log($"Найдено изображений: {backgroundImages.Count}");
        }

        // Запускаем всю последовательность
        StartCoroutine(CompleteStartSequence());

    }

    IEnumerator CompleteStartSequence()
    {
        // Включаем фон
        backgroundImages[4].gameObject.SetActive(true);

        // Ждем 10 секунд
        yield return new WaitForSeconds(3f);

        // Запускаем первый диалог
        dialoglines = new string[] { "Подожди секунду.....", "Сейчас я включу свет!", "И вот!" };

        StartDialogueManually();

        // Выключаем фон
        backgroundImages[4].gameObject.SetActive(false);

        // Меняем реплики
        dialoglines = new string[] { "Привет!", "Проверкааовфылдаоывжа", "лпфвалплыоавпжывап" };

        StartDialogueManually();
    }

    // Update is called once per frame
    void Update()
    {
        // Для тестирования - запуск диалога по нажатию клавиши
        if (Input.GetKeyDown(KeyCode.T) && !dialogStarted)
        {
            StartDialogueManually();
        }
    }

    IEnumerator StartDialogAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeDialogue);

        if (!dialogStarted && DialogManager.Instance != null)
        {
            DialogManager.Instance.StartDialog(dialoglines);
            dialogStarted = true; 
        }
    }

    // Метод для тестирования (можно вызвать из других скриптов)
    public void StartDialogueManually()
    {
        if (!dialogStarted && DialogManager.Instance != null)
        {
            DialogManager.Instance.StartDialog(dialoglines);
            dialogStarted = true;
        }
    }
}

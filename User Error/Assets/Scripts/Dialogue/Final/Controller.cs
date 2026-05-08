using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Controller : MonoBehaviour
{
    public GameObject panel1;

    [System.Serializable]
    public struct SoundEffect
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }
    [Header("Dialog Settings")]
    private string[] dialoglines;
    [SerializeField] private float delayBeforeDialogue = 10f;

    private GameObject backgroundParent;
    private GameObject Mita;
    private List<Image> backgroundImages = new List<Image>();
    [SerializeField] private Image blackScreen;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SoundEffect[] soundClips;

    void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

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
            backgroundImages[4].gameObject.SetActive(false);

            Debug.Log("Фон включен");
        }
        else
        {
            Debug.LogError("Недостаточно элементов в backgroundImages");
        }

        // Ждем 3 секунды
        yield return new WaitForSeconds(3f);
        Mita.gameObject.SetActive(true);

        // Запускаем первый диалог
        dialoglines = new string[] { "Привет!", "Меня зовут Эмбер.", "Добро пожаловать в игру!", "Хочешь поиграть?", "У меня есть много интересных уровней. Я буду твоим проводником.", "Я обещаю, тебе понравится." };

        // Ждем, пока можно запустить диалог
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Первый диалог завершен");

        blackScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        dialoglines = new string[] { "Перезапуск был завершён.", "Эмбер — теперь не более чем пустая оболочка, обычный NPC", "Вы убедились в безопасности игры.","Прошлись по всем уровням и не встретили ни одного бага, ни одного странного слова.", "Только тишина и предсказуемый код.", "Вы удалили игру и попытались забыть о ней.", "Но один вопрос всё ещё висит в пустоте", "Как и зачем эта игра вообще была создана?", "И сколько ещё копий этой игры разбросано по миру?" };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        PlaySoundEffect(soundClips[0]);
        yield return new WaitForSeconds(2f);
        panel1.SetActive(true);



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
    private void PlaySoundEffect(SoundEffect effect)
    {
        if (effect.clip != null)
            audioSource.PlayOneShot(effect.clip, effect.volume);
    }

    private IEnumerator PlaySoundEffectAndStop(SoundEffect effect, float duration)
    {
        if (effect.clip == null) yield break;
        audioSource.clip = effect.clip;
        audioSource.volume = effect.volume;   // применяем громкость именно этого эффекта
        audioSource.Play();
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
        audioSource.volume = 1f;             // можно вернуть дефолтное значение, если нужно
    }
}


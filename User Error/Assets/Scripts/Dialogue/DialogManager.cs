using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.03f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode continueKey = KeyCode.Space;
    [SerializeField] private KeyCode skipKey = KeyCode.Return;

    [Header("Typing Sound")]
    [SerializeField] private bool playTypingSound = true;
    [SerializeField] private SoundVariant[] typingSounds;  // можешь добавить несколько
    [SerializeField] private AudioSource typingAudioSource; // ссылка на AudioSource
    [SerializeField] private int playSoundEveryNthLetter =2;

    private Queue<string> sentences;
    private bool isDialogActive = false;
    private bool isTyping = false;
    private string currentSentence;
    private Coroutine typingCoroutine;

    [System.Serializable]
    public class SoundVariant
    {
        public AudioClip clip;
        [Range(0f, 2f)] public float volume = 1f;  // 1 — исходная громкость клипа
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        sentences = new Queue<string>();
        dialogPanel.SetActive(false);

        // Если AudioSource не назначен — попытаемся найти на этом объекте
        if (typingAudioSource == null)
            typingAudioSource = GetComponent<AudioSource>();

        // Если всё ещё нет — создадим и настроим принудительно (опционально)
        if (typingAudioSource == null && playTypingSound)
        {
            typingAudioSource = gameObject.AddComponent<AudioSource>();
            typingAudioSource.playOnAwake = false;
            typingAudioSource.loop = false;
            typingAudioSource.spatialBlend = 0f; // 2D звук
        }
    }

    void Update()
    {
        if (!isDialogActive) return;

        if (Input.GetKeyDown(continueKey) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
                CompleteSentence();
            else
                DisplayNextSentence();
        }

        if (Input.GetKeyDown(skipKey))
            EndDialog();
    }

    public void StartDialog(string[] dialogLines)
    {
        if (isDialogActive) return;

        sentences.Clear();
        foreach (string sentence in dialogLines)
            sentences.Enqueue(sentence);

        dialogPanel.SetActive(true);
        isDialogActive = true;
        DisablePlayerControl();
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialog();
            return;
        }

        currentSentence = sentences.Dequeue();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogText.text = "";
        int letterCounter = 0; // счётчик обработанных букв

        foreach (char letter in sentence)
        {
            dialogText.text += letter;

            if (playTypingSound && letter != ' ')
            {
                letterCounter++;
                if (letterCounter % playSoundEveryNthLetter == 0) // каждый N-й раз
                    PlayTypingSound();
            }

            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    /// <summary>
    /// Выбирает случайный звук из массива и проигрывает его через PlayOneShot.
    /// Не прерывает предыдущий, чтобы звуки могли накладываться как настоящая печать.
    /// </summary>
    private void PlayTypingSound()
    {
        if (typingSounds == null || typingSounds.Length == 0 || typingAudioSource == null)
            return;

        // Случайный вариант
        SoundVariant variant = typingSounds[Random.Range(0, typingSounds.Length)];

        if (variant.clip == null)
            return;

        float randomPitch = Random.Range(0.95f, 1.05f);
        typingAudioSource.pitch = randomPitch;
        // Передаём громкость варианта (она умножается на громкость AudioSource)
        typingAudioSource.PlayOneShot(variant.clip, variant.volume);
    }

    void CompleteSentence()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogText.text = currentSentence;
        isTyping = false;
        StopAllCoroutines();

        // Звуки не проигрываем, т.к. текст появился мгновенно
    }

    void EndDialog()
    {
        StopAllCoroutines();
        dialogPanel.SetActive(false);
        isDialogActive = false;
        EnablePlayerControl();
    }

    public void DisablePlayerControl()
    {
        Hero player = FindObjectOfType<Hero>();
        if (player != null)
        {
            player.enabled = false;
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }

    public void EnablePlayerControl()
    {
        Hero player = FindObjectOfType<Hero>();
        if (player != null) player.enabled = true;
    }

    public bool IsDialogActive() => isDialogActive;
}

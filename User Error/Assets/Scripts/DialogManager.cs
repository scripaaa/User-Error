using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }


    [Header("UI References")]
    [SerializeField] private GameObject dialogpanel;
    [SerializeField] private TextMeshProUGUI dialogText;

    [Header("Settings")]
    [SerializeField] private float textSpeed = 0.03f; // скорость появления текста

    [Header("Input Settings")]
    [SerializeField] private KeyCode continueKey = KeyCode.Space;
    [SerializeField] private KeyCode skipKey = KeyCode.Return;

    private Queue<string> sentences;
    private bool isDialogActive = false;
    private bool isTyping = false;
    private string currentSentence;
    private Coroutine typingCorountine;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sentences = new Queue<string>();
        dialogpanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDialogActive) return;


        if (Input.GetKeyDown(continueKey) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
                CompleteSentence();
            else DisplayNextSentence();
        }

        if (Input.GetKeyDown(skipKey))
            EndDialog();
        
    }

    /// <summary>
    /// Метод начала диалога
    /// </summary>
    /// <param name="dialoglines"> Сюда надо впихнуть весь диалог</param>
    public void StartDialog(string[] dialoglines)
    {
        if (isDialogActive) return;

        sentences.Clear();

        foreach(string sentence in dialoglines)
            sentences.Enqueue(sentence);

        dialogpanel.SetActive(true);
        isDialogActive = true;

        DisablePlayerControl();

        DisplayNextSentence();
    }
    /// <summary>
    /// Выводит следующую реплику
    /// </summary>
    public void DisplayNextSentence()
    {
        if (sentences.Count ==0)
        {
            EndDialog();
            return;
        }

        currentSentence = sentences.Dequeue();

        if (typingCorountine != null)
            StopCoroutine(typingCorountine);

        typingCorountine = StartCoroutine(TypeSentence(currentSentence));
    }

    /// <summary>
    /// Метод, который выводи текст на печать
    /// </summary>
    /// <param name="sentence"> Предложение, которое надо вывести</param>
    /// <returns></returns>
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogText.text = "";

        foreach(char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(textSpeed);

        }
        isTyping = false;

    }
    /// <summary>
    /// Завершает предложение
    /// </summary>
    void CompleteSentence()
    {
        if (typingCorountine != null )
        {
            StopCoroutine(typingCorountine);
        }

        dialogText.text = currentSentence;
        isTyping = false;

        StopAllCoroutines();
    }

    /// <summary>
    /// Заканчивает диалог
    /// </summary>
    void EndDialog()
    {
        StopAllCoroutines();

        dialogpanel.SetActive(false);
        isDialogActive = false;

        EnablePlayerControl();
    }

    /// <summary>
    /// Забирает контроль у игрока
    /// </summary>
    void DisablePlayerControl()
    {
        Hero player = FindObjectOfType<Hero>();
        if (player != null )
        {
            player.enabled = false;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void EnablePlayerControl()
    {
        Hero player = FindObjectOfType<Hero>();
        if (player != null)
        {
            player.enabled = true;
        }
    }

    public bool IsDialogActive()
    {
        return isDialogActive;
    }




}

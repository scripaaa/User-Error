using UnityEngine;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Dialog Settings")]
    [SerializeField] private string[] dialoglines;
    [SerializeField] private float delayBeforeDialogue = 10f;

    private bool dialogStarted = false;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Доделать 
        StartCoroutine(StartDialogAfterDelay());   
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

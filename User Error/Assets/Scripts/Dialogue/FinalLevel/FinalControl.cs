using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;

public class FinalControl : MonoBehaviour
{

    [System.Serializable]
    public struct SoundEffect
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }
    [Header("Dialog Settings")]
    private string[] dialoglines;
    private GameObject Mita;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SoundEffect[] soundClips;



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
        dialoglines = new string[] { "Стой! Подожди!", "Откуда ты знаешь..."};
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.5f);
        PlaySoundEffect(soundClips[0]);
        yield return new WaitForSeconds(1.5f);


        SceneManager.LoadScene("SecondFinall");




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

    private void PlaySoundEffect(SoundEffect effect)
    {
        if (effect.clip != null)
            audioSource.PlayOneShot(effect.clip, effect.volume);
    }
}

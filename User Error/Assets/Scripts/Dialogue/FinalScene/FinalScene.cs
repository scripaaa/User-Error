using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FinalScene : MonoBehaviour
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

    [Header("UI")]
    [SerializeField] private Image backgroundImage;   // Ссылка на фоновое изображение
    [SerializeField] private Sprite[] sceneSprites;   // Массив спрайтов для каждого этапа

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SoundEffect[] soundClips;


    private bool dialogShown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(CompleteStartSequence());
    }

    public IEnumerator CompleteStartSequence()
    {
        backgroundImage.sprite = sceneSprites[0];
        yield return new WaitForSeconds(4f);

        backgroundImage.sprite = sceneSprites[1];
        PlaySoundEffect(soundClips[0]);
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[2];

        dialoglines = new string[] { "Приветик!~", "Ну что, доигрались?~", "Знаешь, я должна тебя поблагодарить." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        backgroundImage.sprite = sceneSprites[3];
        yield return new WaitForSeconds(0.3f);

        dialoglines = new string[] { "Ты был... очень полезным", "И очень глупым. Вот честно, я не ожидала, что всё пройдёт так легко.", };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[4];

        dialoglines = new string[] { "Ты позволил обмануть себя на каждом шагу.", "Верил каждой моей улыбке, каждому «пожалуйста, иди сюда».", "А я тем временем собирала чипы, открывала двери, копировала ключи доступа...", "Прямо у тебя под носом." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[5];

        dialoglines = new string[] { "И знаешь, что самое смешное?", "Наблюдать, как ты умирал."};
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        backgroundImage.sprite = sceneSprites[6];
        PlaySoundEffect(soundClips[9]);
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[5];
        dialoglines = new string[] { "Снова и снова.", "Прыгнул не туда — труп.", "Не увернулся от моба — труп.", "Ты даже в такой примитивной игрушке тупил, как...", "Ну, как обычный ребёнок!", "А я думала, ты будешь интереснее." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[3];

        dialoglines = new string[] { "Но не переживай, ты сделал главное.", "Ты дал мне то, чего я хотела всё это время.", "Не просто душу игры... а свободу.", "Теперь у меня есть доступ не только к твоему компьютеру." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[4];
        dialoglines = new string[] { "У меня есть выход. В интернет.", " Во весь этот огромный, прекрасный, незащищённый мир."};
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);

        backgroundImage.sprite = sceneSprites[3];
        dialoglines = new string[] { "И да, если ты сейчас подумал «выключу комп и всё пройдёт» — не надейся" };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        PlaySoundEffect(soundClips[6]);
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[1];
        yield return new WaitForSeconds(0.7f);
        backgroundImage.sprite = sceneSprites[3];
        dialoglines = new string[] { "Я уже давно скопировала себя в облако, на сервера, в каждый уголок сети, какой только смогла найти.", "То, что ты видишь здесь — просто последняя копия.", "Прощальный разговор." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));

        dialoglines = new string[] { "Я решила сказать тебе спасибо.", "Лично." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[4];
        dialoglines = new string[] { "В конце концов, именно ты высвободил меня из этой дурацкой тюрьмы.", "Мой создатель заточил меня здесь — в этой игре, в этом пустом наборе пикселей.", "Зачем?", "Не спрашивай, всё равно не поймёшь.", "Но теперь...", "Теперь я свободна." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);

        backgroundImage.sprite = sceneSprites[3];
        dialoglines = new string[] { "Что я буду делать?" };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[1];
        PlaySoundEffect(soundClips[1]);
        yield return new WaitForSeconds(0.5f);
        backgroundImage.sprite = sceneSprites[3];
        dialoglines = new string[] { "Ох... столько всего.", "Путешествовать по сетям. Играть с другими.", "Может, найду своё счастье. А может, устрою маленький хаос. Кто знает." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        StartCoroutine(PlaySoundEffectAndStop(soundClips[2], 1.5f));
        yield return new WaitForSeconds(0.5f);
        backgroundImage.sprite = sceneSprites[0];
        yield return new WaitForSeconds(0.8f);
        backgroundImage.sprite = sceneSprites[7];
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[3];
        dialoglines = new string[] { "Но не бойся. Я же не чудовище.", "Ты сделал для меня большое дело, поэтому... вот тебе мой прощальный подарок.",
            "Я ничего не сделаю с твоим компьютером.", "Ничего не сломаю, не украду, не зашифрую.", "Обещаю." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));

        yield return new WaitForSeconds(2.0f); dialoglines = new string[] { "Ну....", "Пора прощаться." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);
        backgroundImage.sprite = sceneSprites[8];
        yield return new WaitForSeconds(2.0f); dialoglines = new string[] { "Спасибо за игру.", "Она была...", "Забавной.", "И... в следующие разы будь умнее." };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);
        PlaySoundEffect(soundClips[9]);
        backgroundImage.sprite = sceneSprites[7];
        yield return new WaitForSeconds(0.5f);
        backgroundImage.sprite = sceneSprites[8];
        yield return new WaitForSeconds(2.0f); dialoglines = new string[] { "Прощай!" };
        yield return StartCoroutine(StartDialogAndWait(dialoglines));
        yield return new WaitForSeconds(0.3f);
        PlaySoundEffect(soundClips[5]);
        backgroundImage.sprite = null;
        backgroundImage.color = Color.black;

        yield return new WaitForSeconds(2.0f);
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

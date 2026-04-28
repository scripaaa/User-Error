using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DeathScreenManager : MonoBehaviour
{
    public static DeathScreenManager Instance { get; private set; }

    [SerializeField] private Image blackScreen;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float delayBeforeRespawn = 0.3f;
    [SerializeField] private float delayAfterRespawn = 0.2f;

    private bool isFading;

    private void Awake()
    {
        Instance = this;
        
        if (blackScreen == null)
        {
            var canvas = new GameObject("DeathBlackScreen_Canvas", typeof(Canvas));
            canvas.transform.SetParent(transform);
            var c = canvas.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = 9999;
            
            var imgObj = new GameObject("DeathBlackScreen", typeof(RectTransform));
            imgObj.transform.SetParent(canvas.transform, false);
            var rt = imgObj.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;

            var img = imgObj.AddComponent<Image>();
            img.color = Color.clear;

            blackScreen = img;
        }
    }

    private void OnEnable()
    {
        if (Instance == null) Instance = this;
    }

    private void OnDisable()
    {
        if (Instance == this) Instance = null;
    }

    public void ShowDeathScreen(System.Action onComplete = null)
    {
        if (isFading) return;
        StartCoroutine(DeathScreenSequence(onComplete));
    }

    private IEnumerator DeathScreenSequence(System.Action onComplete)
    {
        isFading = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeInDuration;
            if (blackScreen != null)
            {
                Color c = blackScreen.color;
                c.a = Mathf.Clamp01(t);
                blackScreen.color = c;
            }
            yield return null;
        }

        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 1f;
            blackScreen.color = c;
        }

        yield return new WaitForSeconds(delayBeforeRespawn);

        onComplete?.Invoke();

        yield return new WaitForSeconds(delayAfterRespawn);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeOutDuration;
            if (blackScreen != null)
            {
                Color c = blackScreen.color;
                c.a = 1f - Mathf.Clamp01(t);
                blackScreen.color = c;
            }
            yield return null;
        }

        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 0f;
            blackScreen.color = c;
        }

        isFading = false;
    }

    public bool IsFading => isFading;
}
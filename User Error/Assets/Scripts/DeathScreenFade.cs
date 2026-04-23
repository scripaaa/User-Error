using UnityEngine;
using UnityEngine.UI;

public class DeathScreenFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float respawnDelay = 0.8f;

    [Header("References")]
    [SerializeField] private Image blackImage;

    private float _fadeTimer;
    private bool _isFadingIn;
    private bool _isFadingOut;
    private bool _isFading;
    private System.Action _onFadeComplete;

    public static DeathScreenFade Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (blackImage == null)
        {
            blackImage = GetComponent<Image>();
        }

        if (blackImage == null && transform.childCount == 0)
        {
            CreateBlackPanel();
        }

        SetupCanvasSorting();

        if (blackImage != null)
        {
            Color c = blackImage.color;
            c.a = 0f;
            blackImage.color = c;
        }
    }

    private void SetupCanvasSorting()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
        }

        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = 1f;
        }

        if (GetComponent<GraphicRaycaster>() == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void CreateBlackPanel()
    {
        GameObject panelObj = new GameObject("DeathScreenBlack");
        panelObj.transform.SetParent(transform);
        panelObj.transform.SetAsFirstSibling();

        RectTransform rect = panelObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        blackImage = panelObj.AddComponent<Image>();
        blackImage.color = Color.black;
    }

    public void TriggerDeathFade(System.Action onRespawn)
    {
        _onFadeComplete = onRespawn;
        StartFadeIn();
    }

    private void StartFadeIn()
    {
        _isFadingIn = true;
        _isFadingOut = false;
        _isFading = true;
        _fadeTimer = 0f;
    }

    private void StartFadeOut()
    {
        _isFadingIn = false;
        _isFadingOut = true;
        _isFading = true;
        _fadeTimer = 0f;
    }

    private void Update()
    {
        if (!_isFading) return;

        float targetAlpha = _isFadingIn ? 1f : 0f;
        float duration = _isFadingIn ? fadeInDuration : fadeOutDuration;

        if (duration <= 0f)
        {
            _fadeTimer = 1f;
        }
        else
        {
            _fadeTimer += Time.unscaledDeltaTime / duration;
        }

        _fadeTimer = Mathf.Clamp01(_fadeTimer);

        float alpha = Mathf.Lerp(_isFadingIn ? 0f : 1f, _isFadingIn ? 1f : 0f, _fadeTimer);

        if (blackImage != null)
        {
            Color c = blackImage.color;
            c.a = alpha;
            blackImage.color = c;
        }

        if (_fadeTimer >= 1f)
        {
            if (_isFadingIn)
            {
                _isFading = false;
                Invoke(nameof(DoRespawn), respawnDelay);
            }
            else
            {
                _isFading = false;
                _onFadeComplete?.Invoke();
            }
        }
    }

    private void DoRespawn()
    {
        _onFadeComplete?.Invoke();
        _onFadeComplete = null;
        StartFadeOut();
    }

    public void SetFadeInDuration(float duration) => fadeInDuration = duration;
    public void SetFadeOutDuration(float duration) => fadeOutDuration = duration;
    public void SetRespawnDelay(float delay) => respawnDelay = delay;
}
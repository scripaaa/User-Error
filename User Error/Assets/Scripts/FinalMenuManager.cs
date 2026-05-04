using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalMenuManager : MonoBehaviour
{
    public static FinalMenuManager Instance { get; private set; }
    public Sprite finalMenuSprite;
    private GameObject menuObject;
    private bool isShown = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowFinalMenu()
    {
        if (isShown) return;
        
        isShown = true;

        GameObject canvasObj = new GameObject("FinalMenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("FinalMenuImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        
        UnityEngine.UI.Image image = imageObj.AddComponent<UnityEngine.UI.Image>();
        if (finalMenuSprite != null)
        {
            image.sprite = finalMenuSprite;
            image.preserveAspect = true;
            image.color = Color.white;
        }
        else
        {
            image.color = Color.black;
        }
        
        RectTransform rect = imageObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        Debug.Log("Final menu shown!");
    }
}
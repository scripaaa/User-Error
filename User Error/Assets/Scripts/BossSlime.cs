using UnityEngine;
using System.Collections;
using UnityEngine.UI;          // ← для Slider
using TMPro;                  // ← если хочешь текст

public class BossSlime : MonoBehaviour
{
    [SerializeField] private int maxHP = 50;
    private int currentHP;
    [SerializeField] private GameObject hpBarRoot;


    [Header("UI")]
    [SerializeField] private Slider hpSlider;              // Слайдер HP босса
    [SerializeField] private TextMeshProUGUI hpText;       // Текст "50 / 50" (необязательно)

    [Header("Drop")]
    public GameObject chipPrefab;
    public Transform dropPoint;

    [Header("Effects")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.2f;

    private SpriteRenderer sprite;
    private Color originalColor;

    private void Awake()
    {
        currentHP = maxHP;
        sprite = GetComponentInChildren<SpriteRenderer>();
        originalColor = sprite.color;

        InitHPUI();
    }

    private void InitHPUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        if (hpText != null)
        {
            hpText.text = currentHP + " / " + maxHP;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        StopAllCoroutines();
        StartCoroutine(FlashEffect());

        UpdateHPUI();

        if (currentHP <= 0)
            Die();
    }
    /*private void HideHPBar()
    {
        if (hpBarRoot != null)
            hpBarRoot.SetActive(false);
    }
    */

    private void UpdateHPUI()
    {
        if (hpSlider != null)
            hpSlider.value = currentHP;

        if (hpText != null)
            hpText.text = currentHP + " / " + maxHP;
    }

    private IEnumerator FlashEffect()
    {
        sprite.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        sprite.color = originalColor;
    }

    private void Die()
    {
        Debug.Log("BOSS DEAD"); // ← временно

        if (hpBarRoot != null)
            hpBarRoot.SetActive(false);

        if (chipPrefab != null && dropPoint != null)
            Instantiate(chipPrefab, dropPoint.position, Quaternion.identity);

        Destroy(gameObject, 0.05f);
    }



}

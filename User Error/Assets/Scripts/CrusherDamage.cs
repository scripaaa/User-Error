using UnityEngine;

public class CrusherDamage : MonoBehaviour
{
    [Header("Crusher Sounds")]
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private AudioClip stoneSound;

    private AudioSource audioSource;
    private bool hasPlayedSound = false;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!hasPlayedSound)
            {
                PlaySounds();
                hasPlayedSound = true;
            }

            if (Hero.Instance != null)
                Hero.Instance.Die();
        }
    }

    private void PlaySounds()
    {
        if (audioSource == null) return;

        if (breakSound != null)
            audioSource.PlayOneShot(breakSound);

        if (stoneSound != null)
            audioSource.PlayOneShot(stoneSound);
    }
}

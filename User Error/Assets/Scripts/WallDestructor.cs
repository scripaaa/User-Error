using UnityEngine;

public class WallDestructor : MonoBehaviour
{
    [Tooltip(" ,      ")]
    public GameObject objectToHide;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        LoadSounds();
    }

    private void LoadSounds()
    {
        // Resources.Load works only in Resources folders. 
        // We will use the serialized fields instead.
    }

    [Header("Sounds")]
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private AudioClip stoneSound;

    /// <summary>
    ///   ,   .
    /// </summary>
    public void DestroyWall()
    {
        //  
        PlaySounds();

        //   ,   
        if (objectToHide != null)
            objectToHide.SetActive(false);

        //   
        Destroy(gameObject);
    }

    private void PlaySounds()
    {
        if (audioSource == null) return;

        if (breakSound != null)
            audioSource.PlayOneShot(breakSound);
        
        if (stoneSound != null)
            audioSource.PlayOneShot(stoneSound);
    }

    // ������ ������������ ��� ����� � ������� (��������, �������)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            DestroyWall();
    }
}

using UnityEngine;

public class CrusherRoot : MonoBehaviour
{
    [Header("Block")]
    public Transform block;
    public float upDistance = 3f;
    public float speed = 5f;
    public float waitTime = 0.5f;

    [Header("Sounds")]
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private AudioClip riseSound;

    private AudioSource audioSource;
    private Vector3 startPos, upPos;
    private bool goingUp = true;
    private float timer = 0f;
    private bool hasHit = false; // ÷òîáû íå èãðàòü çâóê íåñêîëüêî ðàç çà îäíî ïàäåíèå

    void Start()
    {
        startPos = block.position;
        upPos = startPos + Vector3.up * upDistance;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (goingUp)
        {
            block.position = Vector3.MoveTowards(block.position, upPos, speed * Time.deltaTime);

            if (Vector3.Distance(block.position, upPos) < 0.01f)
            {
                timer += Time.deltaTime;
                if (timer >= waitTime)
                {
                    goingUp = false;
                    hasHit = false; // ñáðîñ — ãîòîâ ê ñëåäóþùåìó óäàðó
                    timer = 0f;

                    if (riseSound != null)
                        audioSource.PlayOneShot(riseSound);
                }
            }
        }
        else
        {
            block.position = Vector3.MoveTowards(block.position, startPos, speed * 2 * Time.deltaTime);

            if (Vector3.Distance(block.position, startPos) < 0.01f)
            {
                if (!hasHit)
                {
                    hasHit = true;
                    if (impactSound != null)
                        audioSource.PlayOneShot(impactSound);
                }

                timer += Time.deltaTime;
                if (timer >= waitTime)
                {
                    goingUp = true;
                    timer = 0f;
                }
            }
        }
    }
}
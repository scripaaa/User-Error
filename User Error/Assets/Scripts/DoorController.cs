using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Animator animator;
    public GameObject chipUI; // ← сюда перетащишь ChipUI

    public void OpenDoor()
    {
        animator.SetTrigger("StartGlitch");
        // Any other door opening logic (sound, disabling collision, etc.)
    }

    public void ActivateGlitchWithDelay(float delay)
    {
        StartCoroutine(DelayCoroutine(delay));
    }

    private IEnumerator DelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // включаем UI
        if (chipUI != null)
            chipUI.SetActive(true);

        animator.SetTrigger("StartGlitch");
    }
}

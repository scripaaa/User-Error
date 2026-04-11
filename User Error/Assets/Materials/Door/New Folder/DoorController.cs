using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Animator animator;

    public void ActivateGlitchWithDelay(float delay)
    {
        StartCoroutine(DelayCoroutine(delay));
    }

    private IEnumerator DelayCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("StartGlitch");
    }
}

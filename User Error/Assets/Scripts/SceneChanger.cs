using UnityEngine;
using UnityEngine.SceneManagement; // обязательно для работы со сценами

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
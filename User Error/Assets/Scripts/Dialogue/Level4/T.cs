using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class T : MonoBehaviour
{

    private GameObject Arrow;

    void Start()
    {
        Arrow = GameObject.Find("Arrow");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {



            Arrow.SetActive(false);
            gameObject.SetActive(false);



        }
    }
}

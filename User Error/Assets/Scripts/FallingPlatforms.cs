using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class FallingTilemapPlatform : MonoBehaviour
{
    public Tilemap tilemap;
    public float fallDelay = 0.5f;
    public float respawnDelay = 2f;

    private void Start()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        // Получаем позицию тайла под игроком
        Vector3 hitPos = Vector3.zero;

        foreach (ContactPoint2D hit in collision.contacts)
        {
            hitPos = hit.point - new Vector2(0.01f, 0.01f);
            Vector3Int cell = tilemap.WorldToCell(hitPos);

            if (tilemap.HasTile(cell))
            {
                StartCoroutine(FallAndRespawn(cell));
            }
        }
    }

    private IEnumerator FallAndRespawn(Vector3Int cell)
    {
        TileBase tile = tilemap.GetTile(cell);

        yield return new WaitForSeconds(fallDelay);

        tilemap.SetTile(cell, null); // исчезает

        yield return new WaitForSeconds(respawnDelay);

        tilemap.SetTile(cell, tile); // восстанавливается
    }
}

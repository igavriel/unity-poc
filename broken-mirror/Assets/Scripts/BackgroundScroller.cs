using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;

    private Transform[] tiles;
    private float tileHeight;
    private Camera mainCamera;

    void Start()
    {
        int tileCount = transform.childCount;
        tiles = new Transform[tileCount];

        for (int i = 0; i < tileCount; i++)
        {
            tiles[i] = transform.GetChild(i);
        }

        // Assume all tiles are the same size
        tileHeight = tiles[0].GetComponent<SpriteRenderer>().bounds.size.y;

        mainCamera = Camera.main;
    }

    void Update()
    {
        float cameraBottom = mainCamera.ViewportToWorldPoint(Vector3.zero).y;

        foreach (Transform tile in tiles)
        {
            Vector3 newPos = tile.position + Vector3.down * scrollSpeed * Time.deltaTime;
            tile.position = new Vector3(tile.position.x, Mathf.Round(newPos.y * 100f) / 100f, tile.position.z);

            float tileTop = tile.position.y + tileHeight / 2f;

            // Only move tile if its top is below the bottom of the camera
            if (tileTop <= cameraBottom)
            {
                float highestY = FindHighestTileY();
                tile.position = new Vector3(tile.position.x, highestY - 0.1f + tileHeight, tile.position.z);
            }
        }
    }

    float FindHighestTileY()
    {
        float highest = tiles[0].position.y;
        foreach (Transform tile in tiles)
        {
            if (tile.position.y > highest)
            {
                highest = tile.position.y;
            }
        }
        return highest;
    }
}

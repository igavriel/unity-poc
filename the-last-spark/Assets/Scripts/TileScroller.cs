using UnityEngine;

public class TileScroller : MonoBehaviour
{
    public GameObject tilePrefab;
    public int tileCount = 10;
    public float tileWidth = 1f;
    public float scrollSpeed = 1f;

    private GameObject[] tiles;

    void Start()
    {
        tiles = new GameObject[tileCount];
        for (int i = 0; i < tileCount; i++)
        {
            Vector3 position = new Vector3(i * tileWidth, 0, 0);
            tiles[i] = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        foreach (GameObject tile in tiles)
        {
            tile.transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

            if (tile.transform.position.x < -tileWidth)
            {
                float rightMost = GetRightMostTileX();
                tile.transform.position = new Vector3(rightMost + tileWidth, 0, 0);
            }
        }
    }

    float GetRightMostTileX()
    {
        float maxX = float.MinValue;
        foreach (GameObject tile in tiles)
        {
            if (tile.transform.position.x > maxX)
                maxX = tile.transform.position.x;
        }
        return maxX;
    }
}

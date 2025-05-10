using System.Collections.Generic;
using UnityEngine;

public class EndlessTilemap : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject tilePrefab;
    public Vector2 tileSize = new Vector2(1, 1);

    private Dictionary<Vector2Int, GameObject> tiles = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int prevCamTilePos;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        UpdateTiles();
    }

    void Update()
    {
        Vector2 camWorldPos = mainCamera.transform.position;
        Vector2Int camTilePos = new Vector2Int(
            Mathf.FloorToInt(camWorldPos.x / tileSize.x),
            Mathf.FloorToInt(camWorldPos.y / tileSize.y)
        );

        if (camTilePos != prevCamTilePos)
        {
            UpdateTiles();
            prevCamTilePos = camTilePos;
        }
    }

    void UpdateTiles()
    {
        // Determine visible area
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        int horizontalTiles = Mathf.CeilToInt(width / tileSize.x) + 2;
        int verticalTiles = Mathf.CeilToInt(height / tileSize.y) + 2;

        Vector2 camWorldPos = mainCamera.transform.position;
        Vector2Int camTileCenter = new Vector2Int(
            Mathf.FloorToInt(camWorldPos.x / tileSize.x),
            Mathf.FloorToInt(camWorldPos.y / tileSize.y)
        );

        HashSet<Vector2Int> neededTiles = new HashSet<Vector2Int>();

        for (int y = -verticalTiles / 2; y <= verticalTiles / 2; y++)
        {
            for (int x = -horizontalTiles / 2; x <= horizontalTiles / 2; x++)
            {
                Vector2Int tilePos = camTileCenter + new Vector2Int(x, y);
                neededTiles.Add(tilePos);

                if (!tiles.ContainsKey(tilePos))
                {
                    Vector3 worldPos = new Vector3(
                        tilePos.x * tileSize.x,
                        tilePos.y * tileSize.y,
                        0
                    );
                    GameObject tile = Instantiate(
                        tilePrefab,
                        worldPos,
                        Quaternion.identity,
                        transform
                    );
                    tiles[tilePos] = tile;
                }
            }
        }

        // Remove unseen tiles
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var pair in tiles)
        {
            if (!neededTiles.Contains(pair.Key))
            {
                Destroy(pair.Value);
                toRemove.Add(pair.Key);
            }
        }
        foreach (var key in toRemove)
            tiles.Remove(key);
    }
}

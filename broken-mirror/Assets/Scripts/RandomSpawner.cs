using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomSpawner : MonoBehaviour
{
    public GameObject[] trianglesPrefabs; // Assign prefabs in Inspector
    public GameObject cubePrefab; // assign the cube prefab in the Inspector
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;

    private BoxCollider2D spawnArea;

    public float slotWidth = 0.5f; // horizontal spacing between cubes in a line

    void Start()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        Utils.AssertObjectNotNull(spawnArea, "RandomSpawner requires a BoxCollider2D component!");

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
            SpawnRandomPrefab();
        }
    }

    void SpawnRandomPrefab()
    {
        if (Random.value < 0.5f)
        {
            SpawnTriangle();
        }
        else
        {
            SpawnCubeLine();
        }
    }

    void SpawnTriangle()
    {
        if (trianglesPrefabs.Length == 0)
            return;

        GameObject prefabToSpawn = trianglesPrefabs[Random.Range(0, trianglesPrefabs.Length)];

        Vector2 min = spawnArea.bounds.min;
        Vector2 max = spawnArea.bounds.max;

        Vector2 spawnPos = new Vector2(
            Random.Range(min.x, max.x),
            spawnArea.bounds.min.y - 0.5f // adjust Y to be beneath the area
        );

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // Random scale
        float randomScale = Random.Range(0.1f, 0.5f);
        spawnedObject.transform.localScale = new Vector3(randomScale, randomScale, 1f);

        // Random color
        SpriteRenderer sr = spawnedObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(Random.value, Random.value, Random.value);
        }

        spawnedObject.transform.rotation = Quaternion.Euler(0, 0, 180f);
        spawnedObject.transform.SetParent(transform);
    }

    void SpawnCubeLine()
    {
        if (cubePrefab == null || spawnArea == null)
            return;

        Vector2 min = spawnArea.bounds.min;
        float startX = min.x;
        float y = min.y - 0.5f;

        // Randomly pick 3 unique indices out of 5
        List<int> indices = new List<int> { 0, 1, 2, 3, 4 };
        List<int> chosen = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            int randIndex = Random.Range(0, indices.Count);
            chosen.Add(indices[randIndex]);
            indices.RemoveAt(randIndex);
        }

        foreach (int index in chosen)
        {
            Vector2 spawnPos = new Vector2(startX + index * slotWidth, y);
            GameObject cube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);
            cube.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            cube.transform.SetParent(transform);
        }
    }

    void OnDrawGizmos()
    {
        BoxCollider2D area = GetComponent<BoxCollider2D>();
        if (area != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(area.bounds.center, area.bounds.size);
        }
    }
}

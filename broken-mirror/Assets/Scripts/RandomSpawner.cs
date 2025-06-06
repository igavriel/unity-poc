using UnityEngine;
using System.Collections;

public class RandomSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // Assign prefabs in Inspector
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;

    private BoxCollider2D spawnArea;

    void Start()
    {
        spawnArea = GetComponent<BoxCollider2D>();
        if (spawnArea == null)
        {
            Debug.LogError("Spawner needs a BoxCollider2D attached!");
            return;
        }

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
        if (prefabs.Length == 0)
            return;

        GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];

        Vector2 min = spawnArea.bounds.min;
        Vector2 max = spawnArea.bounds.max;

        Vector2 spawnPos = new Vector2(
            Random.Range(min.x, max.x),
            spawnArea.bounds.min.y - 0.5f // adjust Y to be beneath the area
        );

        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        spawnedObject.transform.rotation = Quaternion.Euler(0, 0, 180f);
        spawnedObject.transform.SetParent(transform);
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

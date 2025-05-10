using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Border")]
    public Sprite wallSprite;
    public Vector2 worldMin = new Vector2(-25.0f, -25.0f); // bottom-left corner
    public Vector2 worldMax = new Vector2(25.0f, 25.0f); // top-right corner
    public Transform worldBoundsContainer;
    public Transform stonesContainer;
    public Transform flamesContainer;
    public Transform enemiesContainer;

    [Header("Spawning")]
    public GameObject stonesPrefab;
    public GameObject flamePrefab;
    public GameObject enemyPrefab;
    public int stonesCount = 50;
    public int flameCount = 20;
    public int enemyCount = 5;

    [Header("Game Timer")]
    public float gameDuration = 120f; // 2 minutes
    private float timer;

    [Header("UI")]
    public Text timerText;
    public GameObject gameOverPanel;

    private int currentScore = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        timer = gameDuration;
        CreateWorldBorders();
        SpawnObjects(stonesPrefab, stonesCount, stonesContainer);
        SpawnObjects(flamePrefab, flameCount, flamesContainer);
        SpawnObjects(enemyPrefab, enemyCount, enemiesContainer);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            GameOver("Time's up!");
        }

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();
    }

    void SpawnObjects(GameObject prefab, int amount, Transform parent)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(worldMin.x, worldMax.x),
                Random.Range(worldMin.y, worldMax.y)
            );
            GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
            obj.transform.SetParent(parent);
        }
    }

    void CreateWorldBorders()
    {
        if (worldBoundsContainer == null || wallSprite == null)
            return;

        float tileWidth = wallSprite.bounds.size.x;
        float tileHeight = wallSprite.bounds.size.y;

        float left = worldMin.x;
        float right = worldMax.x;
        float bottom = worldMin.y;
        float top = worldMax.y;

        // Bottom and Top borders
        for (float x = left; x <= right; x += tileWidth)
        {
            CreateWallTile(new Vector2(x, bottom - tileHeight / 2));
            CreateWallTile(new Vector2(x, top + tileHeight / 2));
        }

        // Left and Right borders
        for (float y = bottom; y <= top; y += tileHeight)
        {
            CreateWallTile(new Vector2(left - tileWidth / 2, y));
            CreateWallTile(new Vector2(right + tileWidth / 2, y));
        }
    }

    void CreateWallTile(Vector2 position)
    {
        GameObject wall = new GameObject("WallTile");
        wall.transform.SetParent(worldBoundsContainer);
        wall.transform.position = position;

        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = wallSprite;
        sr.sortingOrder = 10;

        BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
        collider.tag = "Wall";
        collider.isTrigger = false;
    }

    public void SpawnEnemy()
    {
        SpawnObjects(enemyPrefab, 1, enemiesContainer);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        Debug.Log("Score: " + currentScore);

        // Optional: Check win condition here (e.g., score == flameCount)
        if (currentScore >= flameCount)
        {
            GameOver("You ignited all the flames!");
        }
    }

    public void GameOver(string reason)
    {
        Debug.Log("Game Over: " + reason);
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}

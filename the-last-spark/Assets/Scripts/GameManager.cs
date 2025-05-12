using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float gameDuration = 120f; // 2 minutes

    [SerializeField] private float timeRemaining;

    [SerializeField] private GameObject player;
    private PlayerLight playerLight;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text hitText;
    public TMP_Text flameText;
    public TMP_Text promptText;

    private bool isGameActive = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        isGameActive = true;
        promptText.enabled = false;
        timeRemaining = gameDuration;

        // Find the player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        player.GetComponentInChildren<SpriteRenderer>().enabled = true;
        playerLight = player.GetComponent<PlayerLight>();
        playerLight.NewGame();

        UpdateCounterDisplay();
        CreateWorldBorders();
        SpawnObjects(stonesPrefab, stonesCount, stonesContainer);
        SpawnObjects(flamePrefab, flameCount, flamesContainer);
        SpawnObjects(enemyPrefab, enemyCount, enemiesContainer);
        StartCoroutine(LightDecayRoutine());
    }

    void Update()
    {
        // update time only if not game over and not paused
        if (isGameActive)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining >= 0f)
            {
                UpdateCounterDisplay();
                UpdateScoreDisplay();
            }
            else
            {
                GameOver("Time's up!");
            }
        }
    }

    void UpdateCounterDisplay()
    {
        int seconds = Mathf.FloorToInt(timeRemaining);
        int minutes = seconds / 60;
        seconds = seconds % 60;
        timerText.text = $"Time Left: {minutes:D2}:{seconds:D2}";
    }

    void SpawnObjects(GameObject prefab, int amount, Transform parent)
    {
        float checkRadius = 0.5f; // Adjust based on object size
        int maxAttempts = 100;
        Debug.Log($"Spawning {amount} objects of type {prefab.name}.");

        int spawned = 0;
        int attempts = 0;
        while (spawned < amount && attempts < maxAttempts * amount)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(worldMin.x, worldMax.x),
                Random.Range(worldMin.y, worldMax.y)
            );
            // Check if area is already occupied
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, checkRadius);

            if (hit == null)
            {
                GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);
                obj.transform.SetParent(parent);
                //Debug.Log($"Spawned {prefab.name} at {spawnPos}");
                spawned++;
            }
            attempts++;
        }

        Debug.Log($"Spawned {spawned} objects out of requested {amount} after {attempts} attempts.");
    }

    public void SpawnEnemy()
    {
        SpawnObjects(enemyPrefab, 1, enemiesContainer);
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

    public void UpdateScoreDisplay()
    {
        int flames = playerLight.GetFlameCount();
        int hits = playerLight.GetHitCount();
        flameText.text = $"Flames: {flames}/{flameCount}";
        hitText.text = $"Hits: {hits}";

        // Optional: Check win condition here (e.g., score == flameCount)
        if (flames >= flameCount)
        {
            GameOver("YOU WON!");
            PromptText("You Win - All the flames were rescued!", 5f);
        }
    }

    public void PromptText(string text, float duration)
    {
        StartCoroutine(HandlePrompt(text, duration));
    }

    private IEnumerator HandlePrompt(string text, float duration)
    {
        Debug.Log(text);
        promptText.enabled = true;
        promptText.text = text;
        yield return new WaitForSeconds(duration);
        promptText.enabled = false;
    }

    public void GameOver(string reason, float delay = 3f)
    {
        StartCoroutine(HandleGameOver("Game Over: " + reason, delay));
    }

    private IEnumerator HandleGameOver(string text, float delay)
    {
        PromptText(text, delay);
        player.GetComponentInChildren<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("1.Opening"); // Load your game over scene
    }

    private IEnumerator LightDecayRoutine()
    {
        while (isGameActive && playerLight != null)
        {
            yield return new WaitForSeconds(2f);
            playerLight.DecreaseLight(0.1f, false);

            if (playerLight.GetCurrentLightRadius() <= 0f)
            {
                GameOver("The spark has faded.");
                yield break;
            }
        }
    }
}

using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
    public int totalCreatedStones = 50;
    public int totalCreatedFlames = 20;
    public int totalCreatedEnemies = 5;

    [Header("Game Timer")]
    [SerializeField]
    private float gameDuration = 120f; // 2 minutes

    [SerializeField]
    private float timeRemaining;

    [SerializeField]
    private GameObject player;
    private PlayerLight playerLight;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text hitText;
    public TMP_Text flameText;
    public TMP_Text promptText;

    private bool isGameActive = false;

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
        SpawnObjects(stonesPrefab, totalCreatedStones, stonesContainer);
        SpawnObjects(flamePrefab, totalCreatedFlames, flamesContainer);
        StartCoroutine(StartSpawnEnemiesRoutine());
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
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                BackToMainMenu();
            }
        }
    }

    void BackToMainMenu()
    {
        SceneManager.LoadScene("1.Opening"); // Load your main menu scene
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
                UnityEngine.Random.Range(worldMin.x, worldMax.x),
                UnityEngine.Random.Range(worldMin.y, worldMax.y)
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
        flameText.text = $"Flames: {flames}/{totalCreatedFlames}";
        hitText.text = $"Hits: {hits}";

        // Optional: Check win condition here (e.g., score == flameCount)
        if (flames >= totalCreatedFlames)
        {
            GameOver("YOU WON! - All the flames were rescued!");
        }
    }

    public void GameOver(string reason, float delay = 30.0f)
    {
        StartCoroutine(HandleGameOver("Game Over: " + reason, delay));
    }

    private IEnumerator HandleGameOver(string text, float delay)
    {
        isGameActive = false; // Stop the game loop
        player.GetComponent<Collider2D>().enabled = false; // Disable player collider
        player.GetComponentInChildren<SpriteRenderer>().enabled = false;

        promptText.enabled = true;
        promptText.text = text + "\n\n" + HandleScore();
        yield return new WaitForSeconds(delay);
        BackToMainMenu();
    }

    string HandleScore()
    {
        int flamesScore = playerLight.GetFlameCount() * 10;
        int hitsDamage = playerLight.GetHitCount() * 5;
        int timeBonus = 0;
        int score = flamesScore - hitsDamage;
        if (playerLight.GetFlameCount() == totalCreatedFlames)
        {
            timeBonus = timeRemaining > 0 ? Mathf.FloorToInt(timeRemaining) : 0;
            score += timeBonus;
        }
        PlayerPrefs.SetInt("Score", score);
        // set high score if this is a new high score
        Debug.Log("Current Score: " + score);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScore = score;
            Debug.Log("New High Score: " + score);
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Flames: {playerLight.GetFlameCount().ToString("D3")} * 10 =  {flamesScore}");
        sb.AppendLine($"Hits:  -{playerLight.GetHitCount().ToString("D3")} *  5 = -{hitsDamage}");
        if (timeBonus > 0)
            sb.AppendLine($"Bonus : {timeBonus}");
        sb.AppendLine($"------------------------------");
        sb.AppendLine($"Score : {score}  /  High Score: {highScore}");
        sb.AppendLine();
        sb.AppendLine($"Press Space to return to the main menu.");
        return sb.ToString();
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

    private IEnumerator StartSpawnEnemiesRoutine()
    {
        yield return new WaitForSeconds(3f);
        SpawnObjects(enemyPrefab, totalCreatedEnemies, enemiesContainer);
    }
}

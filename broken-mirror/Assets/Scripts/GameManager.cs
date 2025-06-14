using System.Collections;
using System.Text;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TMP_Text hitsText;
    public TMP_Text timerText;
    public TMP_Text gameOverText;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    private int hitsCount = 0;
    private float timer = 0f;
    private int highScoreTimer = 0;
    private bool isGameActive = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Utils.AssertObjectNotNull(scoreText, "Score Text is not assigned in the inspector.");
        Utils.AssertObjectNotNull(highScoreText, "High Score Text is not assigned in the inspector.");
        Utils.AssertObjectNotNull(hitsText, "Hits Text is not assigned in the inspector.");
        Utils.AssertObjectNotNull(timerText, "Timer Text is not assigned in the inspector.");
        Utils.AssertObjectNotNull(gameOverText, "Game Over Text is not assigned in the inspector.");

        LoadProgress();
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameActive)
        {
            timer += Time.deltaTime;
            UpdateScoreUI();
        }
    }

    void UpdateScoreUI()
    {
        // int seconds = Mathf.FloorToInt(timer);
        // int minutes = seconds / 60;
        // seconds = seconds % 60;
        // timerText.SetText($"{minutes:D2}:{seconds:D2}");
        timerText.text = "Time: " + Mathf.FloorToInt(timer).ToString("D3");
        hitsText.text = "Hits: " + hitsCount;
    }

    void UpdateGameOverUI()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Score");
        sb.AppendLine();
        sb.AppendLine("Hits " + hitsCount.ToString("D3"));
        sb.AppendLine();
        sb.AppendLine("Time " + Mathf.FloorToInt(timer).ToString("D3"));
        scoreText.text = sb.ToString();

        sb.Clear();
        sb.AppendLine("High");
        sb.AppendLine();
        sb.AppendLine(highScoreTimer.ToString("D3"));
        highScoreText.text = sb.ToString();
    }

    void ShowHideUI()
    {
        gameOverText.gameObject.SetActive(!isGameActive);
        scoreText.gameObject.SetActive(!isGameActive);
        highScoreText.gameObject.SetActive(!isGameActive);
    }

    public void StartGame() => StartCoroutine(StartGameRoutine());

    public void GameOver() => StartCoroutine(GameOverRoutine());

    private IEnumerator StartGameRoutine()
    {
        isGameActive = true;
        timer = 0f;
        hitsCount = 0;

        UpdateScoreUI();
        ShowHideUI();

        yield return new WaitForSeconds(1.0f);
    }

    private IEnumerator GameOverRoutine()
    {
        isGameActive = false;
        if (timer > highScoreTimer)
        {
            highScoreTimer = Mathf.FloorToInt(timer);
            highScoreText.text = "High Score: " + highScoreTimer;
            SaveProgress();
        }
        UpdateGameOverUI();
        ShowHideUI();

        yield return new WaitForSeconds(3.0f);
        isGameActive = true;
        ShowHideUI();
        SceneManager.LoadScene("0-Splash");
    }

    private void SaveProgress()
    {
        Debug.Log("Saving game progress...");
        PlayerPrefs.SetInt("highScoreTimer", highScoreTimer);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        Debug.Log("Loading game progress...");
        highScoreTimer = PlayerPrefs.GetInt("highScoreTimer", 0);
    }
}

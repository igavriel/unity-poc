using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    public TMP_Text hitsText;
    public TMP_Text timerText;
    public TMP_Text gameOverText;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    void Start()
    {
        Utils.AssertObjectNotNull(scoreText, "Score Text is not assigned in the inspector.");
        Utils.AssertObjectNotNull(highScoreText, "High Score Text is not assigned in the inspector.");
        Utils.AssertObjectNotNull(hitsText, "Hits Text is not assigned in the inspector.");
        Utils.AssertObjectNotNull(timerText, "Timer Text is not assigned in the inspector.");
        Utils.AssertObjectNotNull(gameOverText, "Game Over Text is not assigned in the inspector.");

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameActive)
        {
            UpdateScoreUI();
        }
        else
        {
            GameOver();
        }
    }

    void UpdateScoreUI()
    {
        // int seconds = Mathf.FloorToInt(timer);
        // int minutes = seconds / 60;
        // seconds = seconds % 60;
        // timerText.SetText($"{minutes:D2}:{seconds:D2}");
        timerText.text = "Time: " + Mathf.FloorToInt(GameManager.Instance.timer).ToString("D3");
        hitsText.text = "Hits: " + GameManager.Instance.hitsCount;
    }

    void UpdateGameOverUI()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Score");
        sb.AppendLine();
        sb.AppendLine("Time " + Mathf.FloorToInt(GameManager.Instance.timer).ToString("D3"));
        //sb.AppendLine();
        //sb.AppendLine("Hits " + GameManager.Instance.hitsCount.ToString("D3"));
        scoreText.text = sb.ToString();

        sb.Clear();
        sb.AppendLine("High");
        sb.AppendLine();
        sb.AppendLine(GameManager.Instance.highScoreTimer.ToString("D3"));
        highScoreText.text = sb.ToString();
    }

    void ShowHideUI()
    {
        bool isGameActive = GameManager.Instance.isGameActive;
        gameOverText.gameObject.SetActive(!isGameActive);
        scoreText.gameObject.SetActive(!isGameActive);
        highScoreText.gameObject.SetActive(!isGameActive);
    }

    public void StartGame() => StartCoroutine(StartGameRoutine());

    public void GameOver() => StartCoroutine(GameOverRoutine());

    private IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(0f);
        UpdateScoreUI();
        ShowHideUI();
    }

    private IEnumerator GameOverRoutine()
    {
        highScoreText.text = "High Score: " + GameManager.Instance.highScoreTimer.ToString("D3");

        UpdateGameOverUI();
        ShowHideUI();

        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("0-Splash");
    }
}

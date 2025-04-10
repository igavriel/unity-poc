using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Canvas canvas;
    public TMP_Text trickText;
    public TMP_Text scoreText;
    public TMP_Text currentScoreText;
    public TMP_Text timerText;
    public float timeRemaining = 90f;
    private bool isGameActive = false;
    private int score = 0;
    private int highScore = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the game state
        score = 0;
        canvas.enabled = true;
        isGameActive = true;
        timeRemaining = 90f; // Set the initial time
        UpdateCounterDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        // update time only if not game over and not paused
        if (isGameActive)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining >= 0)
            {
                UpdateCounterDisplay();
            }
            else
            {
                StartCoroutine(HandleGameOver());
            }
        }
    }

    public void UpdateTrickText(string trick, int score)
    {
        StartCoroutine(HandleLastTrick(trick, score));
        // Update the score text
        this.score += score;
        scoreText.SetText(this.score.ToString());
        // Update high score if necessary
        if (this.score > highScore)
        {
            highScore = this.score;
            //TODO set("HighScore", highScore);
        }
    }

    private System.Collections.IEnumerator HandleLastTrick(string trick, int score)
    {
        // Update the score and trick
        currentScoreText.SetText($"+{score}");
        trickText.SetText(trick);
        yield return new WaitForSeconds(1.0f);
        // Clear the trick and score text
        currentScoreText.SetText("");
        trickText.SetText("");
    }

    void UpdateCounterDisplay()
    {
        int seconds = Mathf.FloorToInt(timeRemaining);
        int minutes = seconds / 60;
        seconds = seconds % 60;
        timerText.SetText($"{minutes:D2}:{seconds:D2}");
    }

    private System.Collections.IEnumerator HandleGameOver()
    {
        isGameActive = false;
        yield return new WaitForSeconds(2f);
        canvas.enabled = false;
        SceneManager.LoadScene("1.Openning"); // Load your game over scene
    }
}

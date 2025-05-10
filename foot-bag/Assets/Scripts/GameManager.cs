using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text trickText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private GameObject player;
    [SerializeField] private float timeRemaining = 90f;

    private bool isGameActive = false;
    private int score = 0;
    private int highScore = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the game state
        score = 0;
        isGameActive = true;
        player.SetActive(true);
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
        gameOverText.enabled = true;
        player.SetActive(false);
        yield return new WaitForSeconds(5f);
        gameOverText.enabled = false;
        SceneManager.LoadScene("1.Opening"); // Load your game over scene
    }
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenScreen : MonoBehaviour
{
    public GameObject helpBox;

    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    // Start is called before the first frame update
    void Start()
    {
        helpBox.SetActive(false);
        LoadScore();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            StartCoroutine(HandleStartGame());
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.SetInt("HighScore", 0);
            LoadScore();
        }
    }

    private IEnumerator HandleStartGame()
    {
        helpBox.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("2.GameScene");
    }

    private void LoadScore()
    {
        int score = PlayerPrefs.GetInt("Score", 0);
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        scoreText.text = "Score: " + score.ToString("D3");
        highScoreText.text = "High Score: " + highScore.ToString("D3");
    }
}

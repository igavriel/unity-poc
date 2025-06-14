using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int hitsCount { get; private set; } = 0;
    public float timer { get; private set; } = 0f;
    public int highScoreTimer { get; private set; } = 0;
    public bool isGameActive { get; private set; } = true;

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
        LoadProgress();
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameActive)
        {
            timer += Time.deltaTime;
        }
    }

    public void StartGame() => StartCoroutine(StartGameRoutine());

    public void GameOver() => StartCoroutine(GameOverRoutine());

    private IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(0f);
        isGameActive = true;
        timer = 0f;
        hitsCount = 0;
    }

    private IEnumerator GameOverRoutine()
    {
        isGameActive = false;
        if (timer > highScoreTimer)
        {
            highScoreTimer = Mathf.FloorToInt(timer);
            SaveProgress();
        }

        yield return new WaitForSeconds(3.0f);
        isGameActive = true;
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

using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public GameObject chooseDifficultyPage;
    public Text scoreText;
    public static GameDifficulty gameDifficulty;

    enum PageState
    {
        None,
        Start,
        Countdown,
        GameOver,
        ChooseDifficulty
    }

    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    int score = 0;
    bool gameOver = true;

    public bool GameOver { get { return gameOver; } }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        scoreText.enabled = false;
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;
    }

    void OnDisable()
    {
        TapController.OnPlayerDied -= OnPlayerDied;
        TapController.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        Parallaxer.ConfigureDifficulty();
        OnGameStarted();
        score = 0;
        gameOver = false;
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString();
    }

    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore");
        if (score > savedScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        scoreText.enabled = false;
        SetPageState(PageState.GameOver);
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                chooseDifficultyPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                chooseDifficultyPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                chooseDifficultyPage.SetActive(false);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                chooseDifficultyPage.SetActive(false);
                break;
            case PageState.ChooseDifficulty:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                chooseDifficultyPage.SetActive(true);   
                break;
        }
    }

    public void ConfirmGameOver()
    {
        SetPageState(PageState.Start);
        scoreText.text = "0";
        OnGameOverConfirmed();
    }

    public void ChooseDifficulty()
    {
        SetPageState(PageState.ChooseDifficulty);
    }

    public void ChooseEasyDifficulty()
    {
        gameDifficulty = GameDifficulty.Easy;
        StartGame();
    }
    public void ChooseMediumDifficulty()
    {
        gameDifficulty = GameDifficulty.Medium;
        StartGame();
    }
    public void ChooseHardDifficulty()
    {
        gameDifficulty = GameDifficulty.Hard;
        StartGame();
    }

    public void StartGame()
    {
        scoreText.enabled = true;
        SetPageState(PageState.Countdown);
    }
}

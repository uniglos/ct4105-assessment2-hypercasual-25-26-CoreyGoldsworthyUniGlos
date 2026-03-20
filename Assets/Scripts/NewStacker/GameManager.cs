using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public GameObject gameOverPanel;
    public GameObject tapToStartPanel;

    public bool IsGameRunning { get; private set; }

    private int score;
    private int bestScore;
    private const string BestScoreKey = "BestScore";


    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        UpdateUI();
        ShowStartScreen();
    }

    void Update()
    {
        if (!IsGameRunning && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
            StartGame();
    }

    

    public void StartGame()
    {
        IsGameRunning = true;
        score = 0;
        UpdateUI();
        SetPanel(tapToStartPanel, false);
        SetPanel(gameOverPanel,   false);
        BlockSpawner.Instance.SpawnNext();
    }

  
    public void AddScore(int points = 1)
    {
        score += points;
        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
        }
        UpdateUI();
    }

   
    public void TriggerGameOver()
    {
        IsGameRunning = false;
        SetPanel(gameOverPanel, true);
        Debug.Log($"[GameManager] Game Over – Score: {score}");
    }

    public void RestartGame() =>
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

 

    void ShowStartScreen()
    {
        SetPanel(tapToStartPanel, true);
        SetPanel(gameOverPanel,   false);
    }

    void UpdateUI()
    {
        if (scoreText     != null) scoreText.text     = score.ToString();
        if (bestScoreText != null) bestScoreText.text = "BEST: " + bestScore;
    }

    static void SetPanel(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }
}

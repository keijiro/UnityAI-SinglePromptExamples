using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private float baseScrollSpeed = 5f;
    
    public float ScrollSpeed { get; private set; }
    
    private Label scoreLabel;
    private VisualElement gameOverPanel;
    private Button restartButton;
    private int score = 0;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        ScrollSpeed = baseScrollSpeed;
    }

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        scoreLabel = root.Q<Label>("ScoreLabel");
        gameOverPanel = root.Q<VisualElement>("GameOverPanel");
        restartButton = root.Q<Button>("RestartButton");

        if (restartButton != null)
        {
            restartButton.clicked += RestartGame;
        }

        UpdateUI();
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        if (isGameOver) return;
        ScrollSpeed = baseScrollSpeed * multiplier;
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        if (score < 0) score = 0; // Prevent negative scores if desired
        UpdateUI();
    }

    public void SubtractScore(int amount)
    {
        if (isGameOver) return;
        score -= amount;
        if (score < 0) score = 0;
        UpdateUI();
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (gameOverPanel != null)
        {
            gameOverPanel.style.display = DisplayStyle.Flex;
        }
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateUI()
    {
        if (scoreLabel != null)
        {
            scoreLabel.text = $"SCORE: {score}";
        }
    }
}

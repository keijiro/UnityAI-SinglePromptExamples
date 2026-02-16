using UnityEngine;
using UnityEngine.UIElements;

public class NumberPuzzleUI : MonoBehaviour
{
    private Label scoreLabel;
    private VisualElement gameOverScreen;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        scoreLabel = root.Q<Label>("ScoreLabel");
        gameOverScreen = root.Q<VisualElement>("GameOverScreen");

        if (gameOverScreen != null)
            gameOverScreen.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnGameOver += ShowGameOver;
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreLabel != null)
            scoreLabel.text = "Score: " + score;
    }

    private void ShowGameOver()
    {
        if (gameOverScreen != null)
            gameOverScreen.style.display = DisplayStyle.Flex;
    }
}

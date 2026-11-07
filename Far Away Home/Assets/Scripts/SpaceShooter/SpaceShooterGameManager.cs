using UnityEngine;
using UnityEngine.UI;

public class SpaceShooterGameManager : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text gameOverText;

    private int score;
    private int lives = 3;
    public bool IsGameOver { get; private set; }

    public void Configure(Text scoreLabel, Text livesLabel, Text gameOverLabel)
    {
        scoreText = scoreLabel;
        livesText = livesLabel;
        gameOverText = gameOverLabel;
        RefreshHud();
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
    }

    private void Start()
    {
        RefreshHud();
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        score += amount;
        RefreshHud();
    }

    public void LoseLife()
    {
        if (IsGameOver) return;
        lives = Mathf.Max(0, lives - 1);
        RefreshHud();

        if (lives == 0)
        {
            IsGameOver = true;
            if (gameOverText != null)
            {
                gameOverText.text = "SHIP DESTROYED\nPress Play to try again";
                gameOverText.gameObject.SetActive(true);
            }
        }
    }

    private void RefreshHud()
    {
        if (scoreText != null) scoreText.text = "SCORE  " + score;
        if (livesText != null) livesText.text = "HULL  " + lives;
    }
}

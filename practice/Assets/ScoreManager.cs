using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public TMP_Text scoreText;

    public int highScore;
    public TMP_Text highScoreText;

    private void Start()
    {
        LoadHighScore();
        UpdateScoreText();
        InvokeRepeating("IncreaseScoreOverTime", 1f, 1f);
    }

    void IncreaseScoreOverTime()
    {
        AddScore(3); // Increase score every second
    }

    public void AddScore(int amount)
    {
        score += amount;
        CheckHighScore(); // Check for new high score every time score updates
        UpdateScoreText();
    }

    void CheckHighScore()
    {
        if (score > highScore) // If current score is higher, update instantly
        {
            highScore = score;
            // Save new high score immediately but only after updating the score
            PlayerPrefs.SetInt("HighScore", highScore);
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }

    void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("High Score Loaded: " + highScore);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ScoreZone"))
        {
            AddScore(5); // Add points when passing an obstacle
        }
    }

    public void GameOver()
    {
        CancelInvoke("IncreaseScoreOverTime"); // Stop auto score increase
        PlayerPrefs.Save(); // Save high score when game ends
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class BreakoutManager : MonoBehaviour
{
    private const int POINTSPERBLOCK = 100;

    [SerializeField]
    TMPro.TMP_Text scoreText;
    [SerializeField]
    TMPro.TMP_Text livesText;

    [SerializeField]
    GameObject gameOverPanel;
    [SerializeField]
    TMPro.TMP_Text endScoreText;

    private int score = 0;
    private int lives = 3;

    private void Start()
    {
        ResetLivesAndScores();
    }

    public void IncreaseScore()
    {
        score += POINTSPERBLOCK;
        scoreText.text = "Score: " + score.ToString();
    }

    public void LoseLife()
    {
        if (lives > 1)
        {
            lives--;
            livesText.text = "Lives: " + lives.ToString();
        }
        else
        {
            GameOver();            
        }
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        endScoreText.text = "Score: " + score.ToString();
    }

    public void Restart()
    {
        gameOverPanel.SetActive(false);
        ResetLivesAndScores();
    }

    private void ResetLivesAndScores()
    {
        lives = 3;
        livesText.text = "Lives: " + lives.ToString();
        score = 0;
        scoreText.text = "Score: " + score.ToString();
    }
}

using UnityEngine;

public class BreakoutManager : MonoBehaviour
{
    private const int POINTSPERBLOCK = 100;

    [SerializeField]
    private BallController ballController;
    [SerializeField]
    private BrickManager brickManager;

    [SerializeField]
    TMPro.TMP_Text scoreText;
    [SerializeField]
    TMPro.TMP_Text livesText;

    [SerializeField]
    GameObject gameOverPanel;
    [SerializeField]
    TMPro.TMP_Text endScoreText;

    [SerializeField]
    private float initalSpeed = 5f;
    [SerializeField]
    private int[] speedIncreaseSteps = { 400, 1200 }; //Holds "score steps" which are basically just scores that trigger a speed increase when our score superscedes them.
    [SerializeField]
    private float speedMultiplierPerIncrease = 1.2f; //How much to increase speed for each increase in speed, this is multiplicative.

    private int score = 0;
    private int lives = 3;
    private int maxBrickLevel = 0; 

    private void Start()
    {
        ballController.speed = initalSpeed;
        ResetLivesAndScores(); //Make sure UI is updated.
    }

    public void IncreaseScore(int brickLevel)
    {
        score += POINTSPERBLOCK;
        scoreText.text = "Score: " + score.ToString();

        CalculateSpeed(brickLevel);
    }

    //Increases speed based on score and the max level brick we have destroyed. This introduces a difficulty curve.
    private void CalculateSpeed(int brickLevel)
    {
        ballController.speed = initalSpeed;

        //Increases speed for each score step (eg 400, 1200) that our score is over.
        for (int i = 0; i < speedIncreaseSteps.Length; i++) //Iterate through all steps and check
        {
            if (score >= speedIncreaseSteps[i])
            {
                ballController.speed *= speedMultiplierPerIncrease;
            }
            else
            {
                break; //If speed is score is lower there is no point checking further score steps as they will all be higher.
            }
        }

        //Increases speed for each brick level we have broken.
        if (brickLevel > maxBrickLevel)
        {
            maxBrickLevel = brickLevel;
        }
        ballController.speed *= Mathf.Pow(speedMultiplierPerIncrease, maxBrickLevel);
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
        gameOverPanel.SetActive(true); //Show game over UI
        endScoreText.text = "Score: " + score.ToString();
    }

    public void Restart()
    {
        gameOverPanel.SetActive(false); //Hide game over UI
        ResetLivesAndScores(); //Make sure UI is updated.

        brickManager.DestroyBricks();
        brickManager.CreateBricks();
    }

    private void ResetLivesAndScores()
    {
        lives = 3;
        livesText.text = "Lives: " + lives.ToString();
        score = 0;
        scoreText.text = "Score: " + score.ToString();
    }
}

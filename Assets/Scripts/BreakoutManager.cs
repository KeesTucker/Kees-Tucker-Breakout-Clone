using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class BreakoutManager : NetworkBehaviour
{
    private const int POINTSPERBLOCK = 100;

    [SerializeField]
    private BrickManager brickManager;
    [HideInInspector]
    public BallController ballController;

    [SerializeField]
    private TMPro.TMP_Text scoreText;
    [SerializeField]
    private TMPro.TMP_Text livesText;

    [SerializeField]
    private Transform wallRight;
    [SerializeField]
    private Transform wallTop;

    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private TMPro.TMP_Text endScoreText;

    public float initalSpeed = 5f;
    [SerializeField]
    private int[] speedIncreaseSteps = { 400, 1200 }; //Holds "score steps" which are basically just scores that trigger a speed increase when our score superscedes them.
    [SerializeField]
    private float speedMultiplierPerIncrease = 1.2f; //How much to increase speed for each increase in speed, this is multiplicative.

    private int score = 0;
    private int lives = 3;
    private int maxBrickLevel = 0; 

    public override void OnStartServer()
    {
        ResetLivesAndScores(); //Make sure UI is updated.
    }

    public override void OnStartClient()
    {
        if (!isServer)
        {
            ResizeCamOnClient();
        }
    }

    private void ResizeCamOnClient()
    {
        float width = wallRight.transform.position.x - 0.5f;
        float height = wallTop.transform.position.y - 0.5f;
        Camera.main.orthographicSize = Mathf.Max(width / Camera.main.aspect, height);
    }

    public void IncreaseScore(int brickLevel, BallController ballController)
    {
        score += POINTSPERBLOCK;
        RpcIncreaseScore(score);
        CalculateSpeed(brickLevel, ballController);
    }

    [ClientRpc]
    private void RpcIncreaseScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    //Increases speed based on score and the max level brick we have destroyed. This introduces a difficulty curve.
    private void CalculateSpeed(int brickLevel, BallController ballController)
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
            RpcLoseLife(lives);
        }
        else
        {
            RpcGameOver(score);            
        }
    }

    [ClientRpc]
    private void RpcLoseLife(int lives)
    {
        livesText.text = "Lives: " + lives.ToString();
    }

    [ClientRpc]
    public void RpcGameOver(int score)
    {
        gameOverPanel.SetActive(true); //Show game over UI
        if (!isServer)
        {
            restartButton.interactable = false;
        }
        endScoreText.text = "Score: " + score.ToString();
    }

    public void Restart()
    {
        RpcRestart();
        ResetLivesAndScores(); //Make sure UI is updated.

        brickManager.DestroyBricks();
        brickManager.CreateBricks();
    }

    [ClientRpc]
    private void RpcRestart()
    {
        gameOverPanel.SetActive(false); //Hide game over UI
        ballController.Reset();
    }

    private void ResetLivesAndScores()
    {
        lives = 3;
        livesText.text = "Lives: " + lives.ToString();
        score = 0;
        scoreText.text = "Score: " + score.ToString();
    }
}

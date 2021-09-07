using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;

public class BreakoutManager : NetworkBehaviour
{
    private const int POINTS_PER_BLOCK = 100;
    private const float BORDER_SIZE = 0.1f;

    [SerializeField]
    private BrickManager brickManager;
    [HideInInspector]
    public BallController ballController;

    //UI references
    [SerializeField]
    private TMPro.TMP_Text scoreText;
    [SerializeField]
    private TMPro.TMP_Text livesText;
    [SerializeField]
    private GameObject gameEndPanel;
    [SerializeField]
    private Button restartButton;
    [SerializeField]
    private TMPro.TMP_Text restartText;
    [SerializeField]
    private TMPro.TMP_Text endScoreText;
    [SerializeField]
    private TMPro.TMP_Text endText;

    //References to collider walls for an easy way to figure out camera size on client. Little bit funky, should probably replace with a more robust system.
    //[WIP]
    [SerializeField]
    private Transform wallRight;
    [SerializeField]
    private Transform wallTop;

    public float initalSpeed = 5f;
    [SerializeField]
    private int[] speedIncreaseSteps = { 400, 1200 }; //Holds "score steps" which are basically just scores that trigger a speed increase when our score superscedes them.
    [SerializeField]
    private float speedMultiplierPerIncrease = 1.2f; //How much to increase speed for each increase in speed, this is multiplicative.

    [HideInInspector]
    public int numBricks; //Total number of bricks.
    [SyncVar]
    private int score = 0;
    [SyncVar]
    private int lives = 3;
    private int maxBrickLevel = 0;

    public override void OnStartClient()
    {
        if (!isServer)
        {
            ResizeCamOnClient(); //Make sure client's camera's are sized to fit grid size.
        }
        else
        {
            Camera.main.orthographicSize = Constants.CAM_SIZE;
        }
        UpdateLivesAndScores(); //Make sure UI is updated
        Camera.main.orthographicSize *= 1f + BORDER_SIZE;
    }

    private void ResizeCamOnClient()
    {
        float width = wallRight.transform.position.x - Constants.COLLIDER_THICKNESS / 2f;
        float height = wallTop.transform.position.y - Constants.COLLIDER_THICKNESS / 2f;
        Camera.main.orthographicSize = Mathf.Max(width / Camera.main.aspect, height);
    }

    private void ResetLivesAndScores()
    {
        lives = 3;
        score = 0;
        UpdateLivesAndScores();
    }

    private void UpdateLivesAndScores()
    {
        livesText.text = "Lives: " + lives.ToString();
        scoreText.text = "Score: " + score.ToString();
    }

    //Server side
    public void IncreaseScore(int brickLevel, BallController ballController)
    {
        score += POINTS_PER_BLOCK;
        //Win condition, means we have destroyed all bricks
        if (score >= POINTS_PER_BLOCK * numBricks)
        {
            RpcEndGame(true, score);
        }
        else //Otherwise just increase score on clients UI and do a speed calculation
        {
            RpcUpdateScore(score);
            CalculateSpeed(brickLevel, ballController);
        }
        
    }

    [ClientRpc]
    private void RpcUpdateScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    //Server side
    public void LoseLife()
    {
        if (lives > 1)
        {
            lives--;
            RpcUpdateLife(lives); //Update UI
        }
        else //Lose condition
        {
            RpcEndGame(false, score);            
        }
    }

    [ClientRpc]
    private void RpcUpdateLife(int lives)
    {
        livesText.text = "Lives: " + lives.ToString();
    }

    [ClientRpc]
    public void RpcEndGame(bool win, int score) //Game end UI function
    {
        ballController.Reset();
        gameEndPanel.SetActive(true); //Show game end UI
        if (win)
        {
            endText.text = "You Won";
        }
        else
        {
            endText.text = "You Lost";
        }
        if (!isServer)
        {
            restartButton.interactable = false;
            restartText.text = "Restart On Host";
        }
        else
        {
            restartButton.interactable = true;
            restartText.text = "Restart";
        }
        endScoreText.text = "Score: " + score.ToString();
    }

    //Server side, only called from restart button which is only interactable on server.
    public void Restart()
    {
        RpcRestart();
        ResetLivesAndScores(); //Update sync vars for new clients.
        //Sync vars don't update fast enough so pass the reset values through.
        RpcUpdateLife(3); 
        RpcUpdateScore(0);

        //Reset bricks
        brickManager.DestroyBricks();
        brickManager.CreateBricks();
    }

    [ClientRpc]
    private void RpcRestart()
    {
        gameEndPanel.SetActive(false); //Hide game over UI
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
}

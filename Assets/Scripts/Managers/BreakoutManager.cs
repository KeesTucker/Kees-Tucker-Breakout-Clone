using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;

public class BreakoutManager : NetworkBehaviour
{
    [SerializeField]
    private BrickManager brickManager;
    [SerializeField]
    private UIHelper uIHelper;
    [HideInInspector]
    public BallController ballController;
    [SerializeField]
    private int pointsPerBlock = 100;
    public float initalSpeed = 5f;
    [SerializeField]
    private int[] speedIncreaseSteps = { 400, 1200 }; //Holds "score steps" which are basically just scores that trigger a speed increase when our score superscedes them.
    [SerializeField]
    private float speedMultiplierPerIncrease = 1.2f; //How much to increase speed for each increase in speed, this is multiplicative.

    [SyncVar]
    private int score = 0;
    [SyncVar]
    private int lives = 3;
    private int maxBrickLevel = 0;

    [HideInInspector]
    public int numBricks; //Total number of bricks.

    public override void OnStartClient()
    {
        uIHelper.UpdateLivesAndScores(lives, score); //Make sure UI is updated
    }

    [Server]
    private void ResetLivesAndScores()
    {
        lives = 3;
        score = 0;
        uIHelper.UpdateLivesAndScores(lives, score);
    }

    [Server]
    public void IncreaseScore(int brickLevel, BallController ballController)
    {
        score += pointsPerBlock;
        //Win condition, means we have destroyed all bricks
        if (score >= pointsPerBlock * numBricks)
        {
            RpcEndGame(true, score);
        }
        else //Otherwise just increase score on clients UI and do a speed calculation
        {
            RpcUpdateLivesAndScore(lives, score);
            CalculateSpeed(brickLevel, ballController);
        }
        
    }

    [Server]
    public void LoseLife()
    {
        if (lives > 1)
        {
            lives--;
            RpcUpdateLivesAndScore(lives, score);
        }
        else //Lose condition
        {
            RpcEndGame(false, score);            
        }
    }

    [Server]
    public void Restart() //Restart button function
    {
        ResetLivesAndScores();

        RpcRestart();

        //Reset bricks
        brickManager.DestroyBricks();
        brickManager.CreateBricks();
    }

    //Increases speed based on score and the max level brick we have destroyed. This introduces a difficulty curve.
    [Server]
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

    [ClientRpc]
    private void RpcUpdateLivesAndScore(int lives, int score)
    {
        uIHelper.UpdateLivesAndScores(lives, score);
    }

    [ClientRpc]
    private void RpcRestart()
    {
        uIHelper.UpdateLivesAndScores(lives, score);
        uIHelper.HideEndPanel();
    }

    [ClientRpc]
    public void RpcEndGame(bool win, int score) //Game end UI function
    {
        ballController.Reset();
        uIHelper.ShowEndPanel(win, isServer, score);
    }
}

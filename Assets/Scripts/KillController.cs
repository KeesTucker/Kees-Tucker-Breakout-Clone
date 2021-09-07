using UnityEngine;
using Mirror;

public class KillController : NetworkBehaviour
{
    [SerializeField]
    private BreakoutManager gameManager;

    private void OnTriggerEnter(Collider collision)
    {
        //If ball passes bottom of screen
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallController ballController = collision.gameObject.GetComponent<BallController>();
            if (ballController.isLocalBall)
            {
                KillPlayer(ballController);
            }
            
        }
    }

    private void KillPlayer(BallController ballController)
    {
        ballController.CmdLoseLife(); //Take a life off player
        ballController.Reset(); //Reset speed of ball etc
    }
}

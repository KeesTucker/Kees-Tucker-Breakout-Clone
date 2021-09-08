using UnityEngine;

public class KillBall : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        //If ball passes bottom of screen
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallController ballController = collision.gameObject.GetComponent<BallController>();
            if (ballController.hasAuthority) //Make sure we are interacting with localPlayer's ball and have authority.
            {
                KillPlayer(ballController);
            }
            
        }
    }

    //We must be localPlayer's ball so we have authority.
    private void KillPlayer(BallController ballController)
    {
        ballController.CmdLoseLife(); //Take a life from player
        ballController.Reset(); //Reset speed of ball etc
    }
}

using UnityEngine;

public class KillController : MonoBehaviour
{
    [SerializeField]
    private BreakoutManager gameManager;

    private void OnTriggerEnter(Collider collision)
    {
        //If ball passes bottom of screen
        if (collision.gameObject.CompareTag("Ball"))
        {
            KillPlayer(collision.gameObject.GetComponent<BallController>());
        }
    }

    private void KillPlayer(BallController ballController)
    {
        gameManager.LoseLife(); //Take a life off player
        ballController.Reset(); //Reset speed of ball etc
    }
}

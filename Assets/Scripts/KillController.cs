using UnityEngine;

public class KillController : MonoBehaviour
{
    [SerializeField]
    private BreakoutManager gameManager;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            KillPlayer(collision.gameObject.GetComponent<BallController>());
        }
    }

    private void KillPlayer(BallController controller)
    {
        gameManager.LoseLife();
        controller.Reset();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillController : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            KillPlayer(collision.gameObject.GetComponent<BallController>());
        }
    }

    private void KillPlayer(BallController controller)
    {
        //Decrease life count
        controller.Reset();
    }
}

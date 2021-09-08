using UnityEngine;
using System.Collections;
using Mirror;

public class PaddleController : NetworkBehaviour
{
    [SerializeField]
    private GameObject ballGO;
    [SerializeField] //Arbitrary units, uses an approximation of friction based on velocity to allow player to move paddle and influence rebound of ball.
    private float friction = 100f;
    [SerializeField]
    private float heightOfPaddle = -5f;
    [SerializeField]
    private float xClamp = 5f; //X value to clamp paddle too.

    [SyncVar]
    private BallController ballController;

    private float oldPaddleX; //Old x coord of cursor/paddle, used for speed calcs.
    [HideInInspector]
    public Vector3 paddleVelocity;

    public override void OnStartServer()
    {
        GameObject ball = Instantiate(ballGO);
        ball.transform.position = new Vector3(0, -100, 0); //Make sure ball doesnt interfere with game on spawn
        NetworkServer.Spawn(ball, gameObject);

        ballController = ball.GetComponent<BallController>();
        ballController.paddleTransform = transform;
        ballController.paddleController = this;
    }

    public override void OnStartClient()
    {
        if(!isLocalPlayer)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.grey;
            spriteRenderer.sortingOrder = -1;
        }
    }

    //Move and calulate friction velocity for ball collisions
    private void Update()
    {
        if (isLocalPlayer)
        {
            //Find mouse position in world.
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Clamp x so platform can't move off the screen and move position to x coordinate of mouse.
            transform.position = new Vector3(Mathf.Clamp(mousePos.x, -xClamp, xClamp), heightOfPaddle, 0);

            //Calculate velocity of player movement for ball to use for friction on collision.
            paddleVelocity = new Vector3(mousePos.x - oldPaddleX, 0, 0) * friction;
            oldPaddleX = mousePos.x;
        }
    }
}

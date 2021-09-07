using UnityEngine;
using Mirror;

public class PaddleController : NetworkBehaviour
{
    [SerializeField]
    private GameObject ballGO;
    [SyncVar]
    private BallController ballController;

    [SyncVar]
    private float heightOfPaddle;
    private float xEdge; //Edge of screen in world space.

    [SerializeField] //Arbitrary units, uses an approximation of friction based on velocity to allow player to move paddle and influence rebound of ball.
    private float friction = 100f;

    private float oldPaddleX; //Old x coord of cursor/paddle, used for speed calcs.
    [HideInInspector]
    public Vector3 paddleVelocity;

    private void Start()
    {
        if (isServer)
        {
            GameObject ball = Instantiate(ballGO);
            ball.transform.position = new Vector3(0, -100, 0); //Make sure ball doesnt interfere with game on spawn
            //Not really ideal to make ball client side as hacks etc could modify its position and the server would accept it. Can't remember if Mirror checks back with the server etc to make sure nothing crazy has happened. Need to look into that.
            //It's the easiest solution for right now though, and I think it's unnecessary to rectify for this test.
            //[WIP]
            NetworkServer.Spawn(ball, gameObject); 
            ballController = ball.GetComponent<BallController>();
            ballController.paddleTransform = transform;
            ballController.paddleController = this;

            int countID = FindObjectsOfType<PaddleController>().Length - 1;
            float heightOffset = 1.5f + 0.75f * countID;
            heightOfPaddle = -Camera.main.ViewportToWorldPoint(new Vector3(0, 1f, 0)).y + heightOffset;
        }

        if (isLocalPlayer)
        {
            //Find x coordinate at the right most part of the screen and then subtract half width of platform, we use this to clamp the x position of platform.
            xEdge = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0, 0)).x - (transform.localScale.x / 2f);
        }
        else //Else gray out other players
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.grey;
            spriteRenderer.sortingOrder = -1;
        }
    }

    //Move and calulate friction velocity for ball collisions
    private void Update()
    {
        if (isLocalPlayer)//Paddle is modified client side, again could make it easy for hacks.
        {
            //Find mouse position in world.
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Clamp x so platform can't move off the screen and move position to x coordinate of mouse.
            transform.position = new Vector3(Mathf.Clamp(mousePos.x, -xEdge, xEdge), heightOfPaddle, 0);

            //Calculate velocity of player movement for ball to use for friction on collision.
            paddleVelocity = new Vector3(mousePos.x - oldPaddleX, 0, 0) * friction;
            oldPaddleX = mousePos.x;
        }
    }
}

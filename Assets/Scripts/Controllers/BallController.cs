using UnityEngine;
using Mirror;

public class BallController : NetworkBehaviour
{
    private BreakoutManager gameManager;

    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private KeyCode releaseKey; //Keycode the user must press to release the ball.
    [SerializeField]
    private float colliderReboundRandomiseMax = 0.1f; //Value to randomise collision rebounds
    [SerializeField]
    private Vector3 startOffset = Vector3.zero; //Offset for respawning ball on paddle.

    [HideInInspector]
    [SyncVar]
    public Transform paddleTransform;
    [HideInInspector]
    [SyncVar]
    public PaddleController paddleController;
    [HideInInspector]
    [SyncVar]
    public float speed; //Linear speed of ball, is adjusted as game progresses.

    [HideInInspector]
    public bool isLocalBall = false; //Are we connected to the localPlayer.
    [HideInInspector]
    public bool released = false; //Keeps track of whether ball is attached to paddle or game has started.
    [HideInInspector]
    public bool releaseFlag = false; //Triggers the Release() method in FixedUpdate() so physics are handled in FixedUpdate() instead of Update().

    private void Start()
    {
        gameManager = FindObjectOfType<BreakoutManager>();
        speed = gameManager.initalSpeed;

        if (paddleController.isLocalPlayer)//Check whether our paddle is the player object
        {
            isLocalBall = true;
            gameManager.ballController = this;
        }
        else //Gray out the other players.
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.grey;
            spriteRenderer.sortingOrder = -1;
        }
    }

    private void Update()
    {
        if (isLocalBall)//Ball position updates happen client side so it's smooth, bad for hacks though, possibly fix this.
        {
            if (Input.GetKeyDown(releaseKey) && !released)
            {
                released = true;
                releaseFlag = true;
            }
            //Make ball follow paddle.
            if (!released && paddleTransform)
            {
                transform.position = paddleTransform.position + startOffset;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isLocalBall)
        {
            //If player just hit release key
            if (releaseFlag)
            {
                Release();
            }
        }
    }

    //Once the collision is over, let's just reset our velocity to make sure it is constant. Then we can destroy the brick we bounced off.
    private void OnCollisionExit(Collision collision)
    {
        if (isLocalBall)
        {
            if (collision.gameObject.CompareTag("Brick"))
            {
                CmdBrickCollision(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("Paddle"))
            {
                PaddleCollision(paddleController.paddleVelocity);
            }
            AddRandomVelocity();
        }
    }

    [Command]
    private void CmdBrickCollision(GameObject other)
    {
        BrickColorGenerator generator = other.GetComponent<BrickColorGenerator>();
        if (generator.armouredBrick) //If its an armoured brick turn it into a normal brick.
        {
            generator.armouredBrick = false;
            generator.RpcSetNormalBrickColour(); //Set it's colour to that of a normal brick.
        }
        else //Otherwise destroy this brick
        {
            gameManager.IncreaseScore(generator.brickLevel, this);
            Destroy(other.gameObject);
        }
        rb.velocity = Vector3.Normalize(rb.velocity) * speed;
    }

    private void PaddleCollision(Vector3 velocityOfCollision)
    {
        rb.velocity = Vector3.Normalize(rb.velocity + velocityOfCollision) * speed; //Impart velocity of paddle on ball to give friction approximation.
    }

    //Randomise rebounds to avoid ball from getting stuck perfectly horizontal or vertical. Not the most elegant [WIP]
    private void AddRandomVelocity()
    {
        float randomisationAmount = Random.Range(-colliderReboundRandomiseMax, colliderReboundRandomiseMax); //Calculate small random value to add to velocity, use same value for x and y as there is no point it being different.
        rb.velocity = Vector3.Normalize(rb.velocity + new Vector3(randomisationAmount, randomisationAmount, 0)) * speed; //Modify velocity
    }

    //Player died, reset their velocity and released flag. Kill controller fires this. We are the localPlayer's ball.
    public void Reset()
    {
        rb.velocity = Vector2.zero;
        released = false;
    }

    //Fire the ball
    private void Release()
    {
        releaseFlag = false;
        //Randomise start vector, just picks a random vector between (-1, 1) and (1, 1) (that equates to 90 degrees).
        rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), 1f, 0f)) * speed;
    }

    [Command]
    public void CmdLoseLife()
    {
        gameManager.LoseLife();
    }
}

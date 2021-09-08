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
    public bool released = false; //Keeps track of whether ball is attached to paddle or game has started.
    [HideInInspector]
    public bool releaseFlag = false; //Triggers the Release() method in FixedUpdate() so physics are handled in FixedUpdate() instead of Update().

    private void Start()
    {
        gameManager = FindObjectOfType<BreakoutManager>();
        speed = gameManager.initalSpeed;
    }

    public override void OnStartAuthority()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<BreakoutManager>();
        }
        gameManager.ballController = this;
    }

    public override void OnStartClient()
    {
        if (!hasAuthority)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.grey;
            spriteRenderer.sortingOrder = -1;
        }
    }

    private void Update()
    {
        if (hasAuthority)
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
        if (hasAuthority)
        {
            //If player just hit release key
            if (releaseFlag)
            {
                releaseFlag = false;
                //Randomise start vector, just picks a random vector between (-1, 1) and (1, 1) (that equates to 90 degrees).
                rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), 1f, 0f)) * speed;
            }
        }
    }

    //Once the collision is over, let's just reset our velocity to make sure it is constant. Then we can destroy the brick we bounced off.
    private void OnCollisionExit(Collision collision)
    {
        if (hasAuthority)
        {
            if (collision.gameObject.CompareTag("Brick"))
            {
                CmdBrickCollision(collision.gameObject);
            }
            else if (collision.gameObject.CompareTag("Paddle"))
            {
                rb.velocity = Vector3.Normalize(rb.velocity + paddleController.paddleVelocity) * speed; //Impart velocity of paddle on ball to give friction approximation.
            }

            float randomisationAmount = Random.Range(-colliderReboundRandomiseMax, colliderReboundRandomiseMax); //Calculate small random value to add to velocity, use same value for x and y as there is no point it being different.
            rb.velocity = Vector3.Normalize(rb.velocity + new Vector3(randomisationAmount, randomisationAmount, 0)) * speed; //Modify velocity
        }
    }

    [Command]
    private void CmdBrickCollision(GameObject other)
    {
        BrickColorGenerator generator = other.GetComponent<BrickColorGenerator>();
        if (generator.armouredBrick) //If its an armoured brick turn it into a normal brick.
        {
            generator.armouredBrick = false;
            generator.RpcSetNormalBrickColor(); //Set it's colour to that of a normal brick.
        }
        else //Otherwise destroy this brick
        {
            gameManager.IncreaseScore(generator.brickLevel, this);
            Destroy(other.gameObject);
        }
        rb.velocity = Vector3.Normalize(rb.velocity) * speed;
    }

    [Command]
    public void CmdLoseLife()
    {
        gameManager.LoseLife();
    }

    //Player died, reset their velocity and released flag. Kill controller fires this. We are the localPlayer's ball.
    [Client]
    public void Reset()
    {
        rb.velocity = Vector2.zero;
        released = false;
    }
}

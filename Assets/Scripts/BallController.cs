using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private BreakoutManager gameManager;
    [SerializeField]
    private BrickManager brickManager;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform paddle;
    [SerializeField]
    private PaddleController paddleController;

    [SerializeField]
    private KeyCode releaseKey; //Keycode the user must press to release the ball.
    [HideInInspector]
    public float speed;

    private Vector3 offset; //Offset for respawning ball on paddle.
    private bool released = false; //Keeps track of whether ball is attached to paddle or game has started.
    private bool releaseFlag = false; //Triggers the Release() method in FixedUpdate() so physics are handled in FixedUpdate() instead of Update().

    //Capture our current offset compared to the paddle for use in respawning.
    private void Start()
    {
        offset = transform.position - paddle.transform.position;
    }

    private void Update()
    {
        //Release ball on space if not yet released.
        if (Input.GetKeyDown(releaseKey) && !released)
        {
            released = true;
            releaseFlag = true;
        }

        //Make ball follow paddle.
        if (!released)
        {
            transform.position = paddle.transform.position + offset;
        }
    }

    private void FixedUpdate()
    {
        //If player just hit release key
        if (releaseFlag)
        {
            Release();
        }
    }

    //Once the collision is over, let's just reset our velocity to make sure it is constant. Then we can destroy the brick we bounced off.
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            BrickInfo info = collision.gameObject.GetComponent<BrickInfo>();
            if (info.armouredBrick) //If its an armoured brick turn it into a normal brick.
            {
                info.armouredBrick = false;
                brickManager.SetBrickColor(info); //Set it's colour to that of a normal brick.
            }
            else //Otherwise destroy this brick
            {
                gameManager.IncreaseScore(info.brickLevel);
                Destroy(collision.gameObject);
            }
            rb.velocity = Vector3.Normalize(rb.velocity) * speed;
        }
        else if (collision.gameObject.CompareTag("Paddle"))
        {
            rb.velocity = Vector3.Normalize(rb.velocity + paddleController.velocity) * speed; //Impart velocity of paddle on ball to give friction approximation.
        }
        
    }

    //Player died, reset their velocity and released flag. Kill controller fires this.
    public void Reset()
    {
        released = false;
        rb.velocity = Vector2.zero;
    }

    //Fire the ball
    private void Release()
    {
        releaseFlag = false;
        //Randomise start vector, just picks a random vector between (-1, 1) and (1, 1) (that equates to 90 degrees).
        rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), 1f, 0f)) * speed;
    }
}

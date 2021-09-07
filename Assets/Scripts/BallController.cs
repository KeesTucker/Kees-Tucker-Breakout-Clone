using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private BreakoutManager gameManager;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform paddle;

    [SerializeField]
    private KeyCode releaseKey;
    [SerializeField]
    private float speed = 10f;

    private Vector3 offset;
    private bool released = false;
    private bool releaseEvent = false;

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
            releaseEvent = true;
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
        if (releaseEvent)
        {
            Release();
        }
    }

    //Once the collision is over, let's just reset our velocity to make sure it is constant. Then we can destroy the brick we bounced off.
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            gameManager.IncreaseScore();
            rb.velocity = Vector3.Normalize(rb.velocity) * speed;
            Destroy(collision.gameObject);
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
        releaseEvent = false;
        //Randomise start vector, just picks a random vector between (-1, 1) and (1, 1) (that equates to 90 degrees).
        rb.velocity = Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), 1f, 0f)) * speed;
    }
}
